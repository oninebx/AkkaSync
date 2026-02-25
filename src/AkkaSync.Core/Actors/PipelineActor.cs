using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Common;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Domain.Workers;
using AkkaSync.Core.Domain.Workers.Events;
using AkkaSync.Core.Notifications;

namespace AkkaSync.Core.Actors
{
  public class PipelineActor : ReceiveActor
  {
    private readonly PipelineId _id;
    private readonly IEnumerable<ISyncSource> _sources;
    private readonly IReadOnlyList<IReadOnlyList<ISyncTransformer>> _transformers;
    private readonly ISyncSink _sink;
    private readonly int _batchSize;
    private readonly IHistoryStore? _historyStore;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;
    private readonly HashSet<WorkerId> _runWorkers = [];
    private readonly ILoggingAdapter _logger = Context.GetLogger();
    public PipelineActor(
      IPluginProvider<ISyncSource> sourceProvider,
      IPluginProvider<ISyncTransformer> transformerProvider,
      IPluginProvider<ISyncSink> sinkProvider,
      IPluginProvider<IHistoryStore>? historyProvider,
      PipelineId id,
      PipelineSpec spec)
    {
      _cancellationTokenSource = new CancellationTokenSource();
      _cancellationToken = _cancellationTokenSource.Token;

      _sources = sourceProvider.Create(spec.SourceProvider, _cancellationToken);
      _transformers = TransformerDagBuilder.Build(transformerProvider.Create(spec.TransformerProvider, _cancellationToken));
      _sink = sinkProvider.Create(spec.SinkProvider, _cancellationToken).First();
      _batchSize = spec.SinkProvider.Parameters.Get<int>("batchSize", 1);

      _historyStore = historyProvider?.Create(spec.HistoryStoreProvider).FirstOrDefault();
      _id = id;

      ReceiveAsync<SharedProtocol.Start>(msg => StartAsync(msg));
      ReceiveAsync<WorkerProtocol.Create>(msg => CreateWorkerAsync(msg));

      ReceiveAsync<WorkerStarted>(msg => HandleStartedWorkerAsync(msg));
      ReceiveAsync<WorkerCompleted>(msg => FinalizeWorker(msg));
      ReceiveAsync<WorkerProgressed>(msg => HandleWorkerProgress(msg));
      ReceiveAsync<WorkerFailed>(msg => HandleFailedWorker(msg));
    }

    protected override void PostStop()
    {
      _cancellationTokenSource.Cancel();
      _cancellationTokenSource.Dispose();
    }

    private async Task StartAsync(SharedProtocol.Start _)
    {
      bool workerCreated = false;
      foreach (var source in _sources)
      {
        string? cursor = default;
        if(_historyStore is IHistoryStore store)
        {
          var record = await store.GetAsync(source.Id, _cancellationToken);
          if (record is SyncHistoryRecord syncRecord && record.ETag == source.ETag && record.Status == "Completed")
          {
            Context.System.EventStream.Publish(new WorkerNonCreationReported(source.Id));
            continue;
          }
          cursor = record?.Cursor;
        }
        
        Self.Tell(new WorkerProtocol.Create(_id, source, cursor));
        workerCreated = true;
      }
      if (!workerCreated)
      {
        Context.Parent.Tell(new PipelineSkipped(_id, "No workers created"));
        Context.Stop(Self);
        return;
      }
      Context.System.EventStream.Publish(new PipelineStartReported(_id));
      _logger.Info($"Pipeline {_id} started successfully.");
    }

    private async Task CreateWorkerAsync(WorkerProtocol.Create msg)
    {
      var source = msg.Source;

      var workerId = new WorkerId(_id, source.Id);
      if (_runWorkers.Contains(workerId))
      {
        return;
      }

      var worker = Context.ActorOf(Props.Create(() => new SyncWorkerActor(workerId, source, _transformers, _sink, _batchSize, msg.Cursor, _cancellationToken)), workerId.ToString());
      
      worker.Tell(new SharedProtocol.Start());
    }

    private async Task FinalizeWorker(WorkerCompleted msg)
    {
      if (_historyStore != null)
      {
        await _historyStore.MarkCompletedAsync(msg.WorkerId.SourceId, msg.Etag);
      }
      Context.System.EventStream.Publish(new WorkerCompleteReported(msg.WorkerId));
      FinalizePipeline(msg.WorkerId);
    }

    private async Task HandleStartedWorkerAsync(WorkerStarted msg)
    {
      if(_historyStore is IHistoryStore store)
      {
        await store.MarkRunningAsync(msg.WorkerId.SourceId);
      }
      _runWorkers.Add(msg.WorkerId);
      _logger.Info("Worker {0} started.", msg.WorkerId);
      Context.System.EventStream.Publish(new WorkerStartReported(msg.WorkerId));
    }

    private async Task HandleWorkerProgress(WorkerProgressed msg)
    {
      _logger.Info($"[Sync Progress] Worker {msg.WorkerId}, Cursor={msg.Cursor}");
      if (_historyStore != null)
      {
        await _historyStore.UpdateCursorAsync(msg.WorkerId.SourceId, msg.Cursor, _cancellationToken);
      }
    }

    private async Task HandleFailedWorker(WorkerFailed msg)
    {
      _logger?.Error($"Worker {msg.WorkerId} failed.");

      if (_historyStore != null)
      {
        await _historyStore.MarkFailedAsync(msg.WorkerId.SourceId, msg.Reason, _cancellationToken);
      }
      FinalizePipeline(msg.WorkerId);
      Context.System.EventStream.Publish(new WorkerFailureReported(msg.WorkerId, msg.Reason));
    }

    private void FinalizePipeline(WorkerId workerId)
    {
      _runWorkers.Remove(workerId);
      if (_runWorkers.Count == 0)
      {
        var pipelineId = workerId.PipelineId;
        Context.Parent.Tell(new PipelineCompleted(pipelineId));
        Context.Stop(Self);
      }
    }
  }
}