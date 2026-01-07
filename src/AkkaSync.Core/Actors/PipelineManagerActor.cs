using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Common;
using AkkaSync.Core.Domain.Pipeline;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.PluginProviders;
using AkkaSync.Core.Runtime;
using AkkaSync.Core.Runtime.PipelineManager;

namespace AkkaSync.Core.Actors;

public class PipelineManagerActor : ReceiveActor
{
  private readonly IPluginProviderRegistry<ISyncSource> _sourceRegistry;
  private readonly IPluginProviderRegistry<ISyncTransformer> _transformerRegistry;
  private readonly IPluginProviderRegistry<ISyncSink> _sinkRegistry;
  private readonly IPluginProviderRegistry<IHistoryStore> _storeRegistry;
  private readonly Dictionary<string, PipelineSpec> _pipelineSpecs;
  private readonly HashSet<string> _completedPipelines = [];
  private PipelineRunGraph _runGraph;
  private int _currentLayerIndex = 0;
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  public PipelineManagerActor(
    IPluginProviderRegistry<ISyncSource> sourceRegistry, 
    IPluginProviderRegistry<ISyncTransformer> transformerRegistry, 
    IPluginProviderRegistry<ISyncSink> sinkRegistry,
    IPluginProviderRegistry<IHistoryStore> storeRegistry,
    PipelineOptions options)
  {
    _sourceRegistry = sourceRegistry;
    _transformerRegistry = transformerRegistry;
    _sinkRegistry = sinkRegistry;
    _storeRegistry = storeRegistry;
    _runGraph = PipelineRunGraph.Create(options.Pipelines);
    _pipelineSpecs = options.Pipelines.ToDictionary(p => p.Name, p => p);

    Receive<PipelineManagerProtocol.Start>(_ =>
    {
      _logger.Info("{0} actor started at {1}.", Self.Path.Name, DateTimeOffset.UtcNow);
      var pipelines = _pipelineSpecs.Values.Select(s => new PipelineInfo(s.Name, s.Schedule)).ToList().AsReadOnly();
      // StartNextLayer();
      Context.System.EventStream.Publish(new PipelineManagerStarted(pipelines));
    });
    Receive<PipelineManagerProtocol.StartPipeline>(msg => CreatePipeline(msg));
    // Receive<PipelineProtocol.Create>(msg => CreatePipeline(msg));
    Receive<PipelineCompleted>(msg => {
      var pipelineName = msg.PipelineId.Name;
      _logger.Info($"Pipeline {msg.PipelineId} completed");
      HandleLayer(pipelineName);
    });

    // Receive<Terminated>(msg =>
    // {
    //   var actorRef = msg.ActorRef;
    //   var pipelineName = actorRef.Path.Name;
      
    //   _logger.Info($"Pipeline {pipelineName} terminated.");
      
    //   // 处理层级调度
    //   HandleLayer(pipelineName);
    // });

    
    // Receive<PipelineStarted>(msg => {
    //   Context.System.EventStream.Publish(msg);
    // });
    // Receive<PipelineCompleted>(msg => {

    //   Context.System.EventStream.Publish(msg);
    // });
    // Receive<StopPipeline>(msg => HandleStopPipeline(msg));
  }

  // protected override void PreStart()
  // {
  //   Context.System.EventStream.Subscribe(Self, typeof(PipelineCompleted));
  // }

  private void StartNextLayer()
  {
    if(_currentLayerIndex >= _runGraph.LayerCount)
    {
      _logger.Info("All pipeline layers have been started.");
      return;
    }
    var layer = _runGraph.Layer(_currentLayerIndex);
    foreach(var name in layer)
    {
      var runId = RunId.New();
      Self.Tell(new PipelineProtocol.Create(runId, name));
    }
  }

  private void CreatePipeline(PipelineManagerProtocol.StartPipeline msg)
  {
    var spec = _pipelineSpecs[msg.Name];
    var runId = RunId.New();
    var actorName = $"{msg.Name}-${runId}";
    if (Context.Child(actorName).IsNobody())
    {
      var source = spec.SourceProvider;
      var sourceProvider = _sourceRegistry.GetProvider(source.Type);

      var transformer = spec.TransformerProvider;
      var transformerChain = _transformerRegistry.GetProvider(transformer.Type);

      var sink = spec.SinkProvider;
      var sinkProvider = _sinkRegistry.GetProvider(sink.Type);

      var store = spec.HistoryStoreProvider;
      var storeProvider = _storeRegistry.GetProvider(store?.Type ?? string.Empty);
      
      if(sourceProvider is not null && transformerChain is not null && sinkProvider is not null)
      {
        var pipelineId = new PipelineId(runId, msg.Name);
        var pipelineActor = Context.ActorOf(Props.Create(() => new PipelineActor(sourceProvider, transformerChain, sinkProvider, storeProvider, pipelineId, spec)), pipelineId.ToString());
        // Context.Watch(pipelineActor);
        pipelineActor.Tell(new PipelineProtocol.Start());
      }
      else
      {
        _logger.Warning($"Failed to create pipeline {spec.Name}. Source, Transformer or Sink provider not found.");
        return;
      }
    }
    else
    {
      _logger.Warning($"Pipeline with ID {spec.Name} already exists.");
    }
  }

  private void HandleLayer(string pipelineName)
  {
    _completedPipelines.Add(pipelineName);
    var currentLayer = _runGraph.Layer(_currentLayerIndex);
    if(_completedPipelines.IsSupersetOf(currentLayer))
    {
      _logger.Info($"All pipelines in layer {_currentLayerIndex} have completed. Starting next layer.");
      _currentLayerIndex++;
      StartNextLayer();
    }
  }

  // private void HandleStopPipeline(StopPipeline msg)
  // {
  //   if(_pipelines.TryGetValue(msg.Name, out var actor))
  //   {
  //     actor.Tell(new StopSync(), Sender);
  //   }
  //   else
  //   {
  //     _logger.Warning($"Pipeline with ID {msg.Name} does not exist.");
  //   }
  // }
}