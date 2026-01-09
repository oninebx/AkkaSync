using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Common;
using AkkaSync.Core.Domain.Pipeline;
using AkkaSync.Core.Domain.Worker;

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

        ReceiveAsync<PipelineProtocol.Start>(msg => StartAsync(msg));
        ReceiveAsync<WorkerProtocol.Create>(msg => CreateWorkerAsync(msg));
        ReceiveAsync<WorkerCompleted>(msg => FinalizeWorker(msg));
        ReceiveAsync<WorkerProgressed>(msg => HandleWorkerProgress(msg));
        ReceiveAsync<WorkerFailed>(msg => HandleFailedWorker(msg));

        // Receive<Terminated>(msg => HandleTerminated(msg));
        // Receive<StopSync>(_ => HandleStop());
      }
      private async Task StartAsync(PipelineProtocol.Start _)
      {
        
        foreach(var source in _sources)
        {
          Self.Tell(new WorkerProtocol.Create(_id, source));
        }
        Context.System.EventStream.Publish(new PipelineStarted(_id));
        _logger.Info($"Pipeline {_id} started successfully.");
      }

      private async Task CreateWorkerAsync(WorkerProtocol.Create msg)
      {
        var source = msg.Source;
        string? cursor = default!;
        if(_historyStore is IHistoryStore store)
        {
          var record = await store.GetAsync(source.Id, _cancellationToken);
          if(record is SyncHistoryRecord syncRecord && record.ETag == source.ETag && record.Status == "Completed")
          {
            return; 
          }
          cursor = record?.Cursor;
          await _historyStore.MarkRunningAsync(source.Id);
        }
        
        var workerId = new WorkerId(_id, source.Id);
        if(_runWorkers.Contains(workerId))
        {
          return;
        }
        
        var worker = Context.ActorOf(Props.Create(() => new SyncWorkerActor(workerId, source, _transformers, _sink, _batchSize, cursor, _cancellationToken)), workerId.ToString());
        _runWorkers.Add(workerId);
        // Context.Watch(worker);
        worker.Tell(new WorkerProtocol.Start());
      }

      private async Task FinalizeWorker(WorkerCompleted msg) {
        if(_historyStore != null)
        {
          await _historyStore.MarkCompletedAsync(msg.WorkerId.SourceId, msg.Etag);
        }
        _runWorkers.Remove(msg.WorkerId);
        
        Context.System.EventStream.Publish(msg);

        if(_runWorkers.Count == 0)
        {
          var pipelineId = msg.WorkerId.PipelineId;
          Context.System.EventStream.Publish(new PipelineCompleted(pipelineId));
          _logger.Info($"Pipeline {pipelineId} completed.");

          Context.Stop(Self);
        }
      }

      // private void HandleStop()
      // {
      //     _logger.Info($"Pipeline Pipeline-{_spec.Name} stopped syncing.");
      //     // Add your stop logic here
      // }

      // private async Task HandleProcessingCompleted(ProcessingCompleted msg)
      // {
      //     _logger.Info($"Pipeline Pipeline-{_spec.Name} completed processing.");
      //     if(_historyStore != null)
      //     {
      //       await _historyStore.MarkCompletedAsync(msg.SourceId, msg.ETag);
      //     }
          
      // }

      private async Task HandleWorkerProgress(WorkerProgressed msg)
      {
        _logger.Info($"[Sync Progress] Worker {msg.WorkerId}, Cursor={msg.Cursor}");
        if(_historyStore != null)
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
      }

      // private void HandleTerminated(Terminated msg)
      // {
      //   var actorName = msg.ActorRef.Path.Name;
      //   _workers.Remove(actorName);
      //   _logger.Info($"Worker terminated. Name={actorName}, Path={msg.ActorRef.Path}");
      //   if(_workers.Count == 0)
      //   {
      //     Context.Stop(Self);
      //   }
      // }

      // private void PrintTablesData(TransformContext context)
      // {
      //     foreach (var table in context.TablesData)
      //     {
      //         _logger.Info($"Table: {table.Key}");
      //         foreach (var column in table.Value)
      //         {
      //             _logger.Info($"  {column.Key}: {column.Value}");
      //         }
      //     }
      // }
    }
}