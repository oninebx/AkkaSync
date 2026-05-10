using Akka.Actor;
using Akka.Event;
using Akka.Streams.Dsl;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Common;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Plugins.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Domain.Workers;
using AkkaSync.Core.Domain.Workers.Events;
using AkkaSync.Core.Notifications;
using OpenTelemetry.Metrics;
using System;

namespace AkkaSync.Infrastructure.Actors
{
  public class PipelineActor : ReceiveActor
  {
    private readonly PipelineId _id;
    private readonly IReadOnlyList<ISyncSource> _sources;
    private readonly IReadOnlyList<IReadOnlyList<ISyncTransform>> _transformers;
    private readonly IReadOnlyList<ISyncSink> _sinks;
    private readonly int _batchSize;
    private readonly IHistoryStore _historyStore;
    private readonly IErrorStore _errorStore;
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;
    private readonly HashSet<WorkerId> _runWorkers = [];
    private readonly Dictionary<string, int> _pluginProcessedCount;
    private readonly Dictionary<string, int> _pluginErrorCount;
    private readonly ILoggingAdapter _logger = Context.GetLogger();
    public PipelineActor(
      IPluginProvider<ISyncSource> sourceProvider,
      IReadOnlyDictionary<string, IPluginProvider<ISyncTransform>> transformProviders,
      IReadOnlyDictionary<string, IPluginProvider<ISyncSink>> sinkProviders,
      IHistoryStore historyStore,
      IErrorStore errorStore,
      PipelineId id,
      int batchSize,
      IReadOnlyList<PluginSpec> specs)
    {
      _cancellationTokenSource = new CancellationTokenSource();
      _cancellationToken = _cancellationTokenSource.Token;

      var sourceSpec = specs.FirstOrDefault(s => s.Type == "source") ?? throw new NullReferenceException("Source Provider cannot be empty");
      _sources = [.. sourceProvider.Create(sourceSpec, _cancellationToken)];

      var transformSpecs = specs.Where(s => s.Type == "transform").ToList();
      _transformers = DagBuilder.Build(transformSpecs.SelectMany(transform => transformProviders[transform.Provider].Create(transform, _cancellationToken)), sourceSpec.Key);

      var sinkSpecs = specs.Where(s => s.Type == "sink").ToList();
      if(sinkSpecs.Count == 0)
      {
        throw new NullReferenceException("Sink Provider cannot be empty");
      }
      _sinks = [.. sinkSpecs.SelectMany(sink => sinkProviders[sink.Provider].Create(sink, _cancellationToken))];
      _batchSize = batchSize;

      _historyStore = historyStore;
      _errorStore = errorStore;
      _id = id;

      _pluginErrorCount = [];
      _pluginProcessedCount = [];

      ReceiveAsync<SharedProtocol.Start>(msg => StartAsync(msg));
      ReceiveAsync<WorkerProtocol.Create>(msg => CreateWorkerAsync(msg));

      ReceiveAsync<WorkerStarted>(msg => HandleStartedWorkerAsync(msg.WorkerId));
      ReceiveAsync<WorkerCompleted>(msg => FinalizeWorker(msg.WorkerId, msg.Etag));
      ReceiveAsync<WorkerProgressed>(msg => HandleWorkerProgress(msg));
      ReceiveAsync<WorkerFailed>(msg => HandleFailedWorker(msg));
      ReceiveAsync<WorkerErrored>(msg => HandleWorkerErrored(msg));
      Receive<PluginsBatchProcessed>(msg => HandleBatchProcessed(msg.Processed, msg.Errors));
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
        var skippedEvent = new PipelineSkipped(_id, "No workers created");
        Context.Parent.Tell(skippedEvent);
        _logger.Info($"Pipeline {_id} skipped for no workers being created.");
        Context.System.EventStream.Publish(skippedEvent);
        Context.Stop(Self);
        return;
      }
      else
      {
        Context.System.EventStream.Publish(new PipelineStarted(_id, ((IEnumerable<IPlugin>)[.. _sources, .. _transformers.SelectMany(t => t), .. _sinks]).ToDictionary(p => p.Id, p => p)));
      }
    }

    private async Task CreateWorkerAsync(WorkerProtocol.Create msg)
    {
      var source = msg.Source;

      var workerId = new WorkerId(_id, source.Id);
      if (_runWorkers.Contains(workerId))
      {
        return;
      }

      var worker = Context.ActorOf(Props.Create(() => new SyncWorkerActor(workerId, source, _transformers, _sinks, _batchSize, msg.Cursor, _cancellationToken)), workerId.ToString());
      
      worker.Tell(new SharedProtocol.Start());
    }

    private async Task FinalizeWorker(WorkerId id, string etag)
    {
      if (_historyStore != null)
      {
        await _historyStore.MarkCompletedAsync(id.SourceId, etag);
      }
      //Context.System.EventStream.Publish(new WorkerCompleteReported(msg.WorkerId));
      FinalizePipeline(id);
    }

    private async Task HandleStartedWorkerAsync(WorkerId id)
    {
      if(_historyStore is IHistoryStore store)
      {
        await store.MarkRunningAsync(id.SourceId);
      }
      _runWorkers.Add(id);
      _logger.Info("Worker {0} started.", id);
      Context.System.EventStream.Publish(new WorkerStartReported(id));
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
      //_logger.Error($"Worker {msg.WorkerId} encountered errors: {string.Join(", ", msg.Errors.Select(e => $"{_id.Key} - {e.PluginId}: {e.Message}"))}");
      await _errorStore.RecordErrorsAsync([.. msg.Errors.Select(e => new ErrorRecord(_id.Key, _id.RunId.ToString(), e.PluginId, e.Message) { Context = e.Context })]);
      Context.System.EventStream.Publish(new WorkerErrorReported(msg.WorkerId, msg.Errors.GroupBy(e => e.PluginId).ToDictionary(g => g.Key, g => g.Count())));
    }

    private void HandleBatchProcessed(Dictionary<string, int> processed, Dictionary<string, int> errors)
    {
      MergeCounts(processed, _pluginProcessedCount);
      MergeCounts(errors, _pluginErrorCount);

      var metrics = _pluginProcessedCount.Keys
        .Union(_pluginErrorCount.Keys)
        .Select(pluginId => new PluginMetrics(pluginId, _pluginProcessedCount.GetValueOrDefault(pluginId), _pluginErrorCount.GetValueOrDefault(pluginId)))
        .ToDictionary(p => p.PluginId);

      Context.System.EventStream.Publish(new PipelineBatchProcessed(metrics));

    }

    private void MergeCounts(Dictionary<string, int> source, Dictionary<string, int> target)
    {
      foreach (var (key, value) in source)
      {
        if (target.TryGetValue(key, out var existing))
        {
          target[key] = existing + value;
        }
        else
        {
          target[key] = value;
        }
      }
    }
  }
}