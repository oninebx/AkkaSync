using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Core.Messging;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.PluginProviders;
using AkkaSync.Messages;
using Microsoft.VisualBasic;

namespace AkkaSync.Core.Actors
{
    public class PipelineActor : ReceiveActor
    {
      private readonly IPluginProvider<ISyncSource> _sourceProvider;
      private readonly IPluginProvider<ISyncTransformer> _transformerProvider;
      private readonly IPluginProvider<ISyncSink> _sinkProvider;
      private readonly IHistoryStore? _historyStore;
      private CancellationTokenSource? _cancellationTokenSource;
      private readonly PipelineContext _context;
      private readonly IDictionary<string, IActorRef> _workers = new Dictionary<string, IActorRef>();
      private readonly ILoggingAdapter _logger = Context.GetLogger();
      public PipelineActor(
        IPluginProvider<ISyncSource> sourceProvider, 
        IPluginProvider<ISyncTransformer> transformerProvider, 
        IPluginProvider<ISyncSink> sinkProvider,
        IPluginProvider<IHistoryStore>? historyProvider,
        PipelineContext context)
      {
          _sourceProvider = sourceProvider;
          _transformerProvider = transformerProvider;
          _sinkProvider = sinkProvider;
          _historyStore = historyProvider?.Create(context.HistoryStoreProvider).FirstOrDefault();
          _context = context;
          ReceiveAsync<StartSync>(_ => HandleStart());
          Receive<StopSync>(_ => HandleStop());
          ReceiveAsync<ProcessingCompleted>(msg => HandleProcessingCompleted(msg));
          ReceiveAsync<ProcessingProgress>(msg => HandleProcessingProgress(msg));
          ReceiveAsync<ProcessingFailed>(msg => HandleProcessingFailed(msg));
          Receive<Terminated>(msg => HandleTerminated(msg));
      }

      private async Task HandleStart()
      {
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;
        var transformerChain = _transformerProvider.Create(_context.TransformerProvider, cancellationToken).First();
        var sink = _sinkProvider.Create(_context.SinkProvider, cancellationToken).First();
        var batchSize = _context.SinkProvider.Parameters.TryGetValue("batchSize", out var s) && int.TryParse(s, out var size) ? size : 1;

        var sources = _sourceProvider.Create(_context.SourceProvider, CancellationToken.None);
        foreach(var source in sources)
        {
          string? cursor = default!;
          if(_historyStore is IHistoryStore store)
          {
            var record = await store.GetAsync(source.Id, cancellationToken);
            if(record is SyncHistoryRecord syncRecord && record.ETag == source.ETag && record.Status == "Completed")
            {
              continue; 
            }
            cursor = record?.Cursor;
            await _historyStore.MarkRunningAsync(source.Id);
          }
          
          var workerName = $"Worker-{source.Id}";
          if(_workers.ContainsKey(workerName))
          {
              continue;
          }
          
          var worker = Context.ActorOf(Props.Create(() => new SyncWorkerActor(source, transformerChain, sink, batchSize, cursor, cancellationToken)), workerName);
          _workers[workerName] = worker;
          Context.Watch(worker);
        }
      }

      private void HandleStop()
      {
          _logger.Info($"Pipeline Pipeline-{_context.Name} stopped syncing.");
          // Add your stop logic here
      }

      private async Task HandleProcessingCompleted(ProcessingCompleted msg)
      {
          _logger.Info($"Pipeline Pipeline-{_context.Name} completed processing.");
          if(_historyStore != null)
          {
            await _historyStore.MarkCompletedAsync(msg.SourceId, msg.ETag);
          }
          
      }

      private async Task HandleProcessingProgress(ProcessingProgress msg)
      {
        _logger.Info($"[Sync Progress] SourceId={msg.SourceId}, Cursor={msg.Cursor}");
        if(_historyStore != null)
        {
          await _historyStore.UpdateCursorAsync(msg.SourceId, msg.Cursor, _cancellationTokenSource?.Token ?? CancellationToken.None);
        }
      }

      private async Task HandleProcessingFailed(ProcessingFailed msg)
      {
        _logger?.Error($"Processing failed for SourceId={msg.SourceId}");

        if (_historyStore != null)
        {
            await _historyStore.MarkFailedAsync(msg.SourceId, msg.Reason, _cancellationTokenSource?.Token ?? CancellationToken.None);
        }
      }

      private void HandleTerminated(Terminated msg)
      {
        var actorName = msg.ActorRef.Path.Name;
          _workers.Remove(actorName);
          if(_workers.Count == 0)
          {
              Context.Parent.Tell(new PipelineCompleted(Self.Path.Name));
          }
          _logger.Info($"Worker terminated. Name={actorName}, Path={msg.ActorRef.Path}");
      }

      private void PrintTablesData(TransformContext context)
      {
          foreach (var table in context.TablesData)
          {
              _logger.Info($"Table: {table.Key}");
              foreach (var column in table.Value)
              {
                  _logger.Info($"  {column.Key}: {column.Value}");
              }
          }
      }
    }
}