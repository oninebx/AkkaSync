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
    private readonly IReadOnlyList<IReadOnlyList<ISyncTransform>> _transformers;
    private readonly ISyncSink _sink;
    private readonly int _batchSize;
    private readonly IHistoryStore _historyStore;
    private readonly IErrorStore _errorStore;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;
    private readonly HashSet<WorkerId> _runWorkers = [];
    private readonly ILoggingAdapter _logger = Context.GetLogger();
    public PipelineActor(
      IPluginProvider<ISyncSource> sourceProvider,
      IReadOnlyDictionary<string, IPluginProvider<ISyncTransform>> transformProviders,
      IPluginProvider<ISyncSink> sinkProvider,
      IHistoryStore historyStore,
      IErrorStore errorStore,
      PipelineId id,
      IReadOnlyList<PluginSpec> specs)
    {
      _cancellationTokenSource = new CancellationTokenSource();
      _cancellationToken = _cancellationTokenSource.Token;

      var sourceSpec = specs.FirstOrDefault(s => s.Type == "source") ?? throw new NullReferenceException("Source Provider cannot be empty");
      _sources = sourceProvider.Create(sourceSpec, _cancellationToken);

      var transformSpecs = specs.Where(s => s.Type == "transform").ToList();
      _transformers = DagBuilder.Build(transformSpecs.SelectMany(transform => transformProviders[transform.Provider].Create(transform, _cancellationToken)), sourceSpec.Key);

      var sinkSpec = specs.FirstOrDefault(s => s.Type == "sink") ?? throw new NullReferenceException("Sink Provider cannot be empty");
      _sink = sinkProvider.Create(sinkSpec, _cancellationToken).First();
      if(sinkSpec.Parameters.TryGetProperty("batchSize", out var batchSizeElement))
      {
        _batchSize = batchSizeElement.GetInt32();
      }
      else
      {
        _batchSize = 1;
      }
      //_batchSize = sinkSpec.Parameters.Get<int>("batchSize", 1);

      _historyStore = historyStore;
      _errorStore = errorStore;
      _id = id;

      ReceiveAsync<SharedProtocol.Start>(msg => StartAsync(msg));
      ReceiveAsync<WorkerProtocol.Create>(msg => CreateWorkerAsync(msg));

      ReceiveAsync<WorkerStarted>(msg => HandleStartedWorkerAsync(msg));
      ReceiveAsync<WorkerCompleted>(msg => FinalizeWorker(msg));
      ReceiveAsync<WorkerProgressed>(msg => HandleWorkerProgress(msg));
      ReceiveAsync<WorkerFailed>(msg => HandleFailedWorker(msg));
      ReceiveAsync<WorkerErrored>(msg => HandleWorkerErrored(msg));
    }

    protected override void PreStart()
    {
      var sourceInstances = _sources.Select(s => new PluginInstance(s.QualifiedId, s.Name, "source", _id.Key)).ToList();
      var transformerInstances = _transformers.Select(layer => (IReadOnlyList<PluginInstance>)[.. layer.Select(t => new PluginInstance(t.QualifiedId, t.Name, "transformer", _id.Key) {
        Dependencies = [.. t.DependsOn.Select(d => d)]
      })])
        .ToList();
      var sinkInstance = new PluginInstance(_sink.QualifiedId, _sink.Name, "sink", _id.Key);

      if (transformerInstances.Count > 0)
      {
        var firstLayer = transformerInstances[0];
        foreach (var t in firstLayer)
        {
          t.Dependencies.AddRange(sourceInstances.Select(s => s.Id));
        }
      }

      if (transformerInstances.Count > 0)
      {
        var lastLayer = transformerInstances[^1];

        sinkInstance.Dependencies.AddRange(lastLayer.Select(t => t.Id));
      }
      else
      {
        sinkInstance.Dependencies.AddRange(sourceInstances.Select(s => s.Id));
      }

      Context.System.EventStream.Publish(new PipelineCreatedReported(_id, sourceInstances, [.. transformerInstances.SelectMany(layer => layer)], sinkInstance));
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
    private async Task HandleWorkerErrored(WorkerErrored msg)
    {
      _logger.Error($"Worker {msg.WorkerId} encountered errors: {string.Join(", ", msg.Errors.Select(e => $"{_id.Key} - {e.PluginId}: {e.Message}"))}");
      await _errorStore.RecordErrorsAsync([.. msg.Errors.Select(e => new ErrorRecord(_id.Key, _id.RunId.ToString(), e.PluginId, e.Message) { Context = e.Context })]);
      Context.System.EventStream.Publish(new WorkerErrorReported(msg.WorkerId, msg.Errors.GroupBy(e => e.PluginId).ToDictionary(g => g.Key, g => g.Count())));
    }
  }
}