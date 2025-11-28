using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Core.Configuration;
using AkkaSync.Core.Messging;
using AkkaSync.Core.Pipeline;
using AkkaSync.Core.PluginProviders;
using AkkaSync.Messages;

namespace AkkaSync.Core.Actors
{
    public class PipelineActor : ReceiveActor
    {
      private readonly IPluginProvider<ISyncSource> _sourceProvider;
      private readonly IPluginProvider<ISyncTransformer> _transformerProvider;
      private readonly IPluginProvider<ISyncSink> _sinkProvider;
      private CancellationTokenSource? _cancellationTokenSource;
      private readonly PipelineContext _context;
      private readonly IDictionary<string, IActorRef> _workers = new Dictionary<string, IActorRef>();
      private readonly ILoggingAdapter _logger = Context.GetLogger();
      public PipelineActor(IPluginProvider<ISyncSource> sourceProvider, IPluginProvider<ISyncTransformer> transformerProvider, IPluginProvider<ISyncSink> sinkProvider, PipelineContext context)
      {
          _sourceProvider = sourceProvider;
          _transformerProvider = transformerProvider;
          _sinkProvider = sinkProvider;
          _context = context;
          Receive<StartSync>(_ => HandleStart());
          Receive<StopSync>(_ => HandleStop());
          Receive<ProcessingCompleted>(msg => HandleProcessingCompleted(msg));
          
      }

      private void HandleStart()
      {
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;
        var transformerChain = _transformerProvider.Create(_context, cancellationToken).First();
        var sink = _sinkProvider.Create(_context, cancellationToken).First();
        var batchSize = _context.SinkProvider.Parameters.TryGetValue("batchSize", out var s) && int.TryParse(s, out var size) ? size : 1;

        var sources = _sourceProvider.Create(_context, CancellationToken.None);
        foreach(var source in sources)
        {
          var workerName = $"Worker-{source.Key}";
          if(_workers.ContainsKey(workerName))
          {
              continue;
          }
          var worker = Context.ActorOf(Props.Create(() => new SyncWorkerActor(source, transformerChain, sink, batchSize, cancellationToken)), workerName);
          _workers[workerName] = worker;
          Context.Watch(worker);
        }
      }

      private void HandleStop()
      {
          _logger.Info($"Pipeline Pipeline-{_context.Name} stopped syncing.");
          // Add your stop logic here
      }

      private void HandleProcessingCompleted(ProcessingCompleted msg)
      {
          _logger.Info($"Pipeline Pipeline-{_context.Name} completed processing.");
          _workers.Remove(msg.Name);
          if(_workers.Count == 0)
          {
              Context.Parent.Tell(new PipelineCompleted(Self.Path.Name));
          }
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