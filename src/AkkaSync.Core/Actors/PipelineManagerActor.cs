using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Core.Abstractions;
using AkkaSync.Core.Common;
using AkkaSync.Core.Configuration;
using AkkaSync.Core.Messging;
using AkkaSync.Core.PluginProviders;
using AkkaSync.Messages;

namespace AkkaSync.Core.Actors;

public class PipelineManagerActor : ReceiveActor
{
  private readonly IPluginProviderRegistry<ISyncSource> _sourceRegistry;
  private readonly IPluginProviderRegistry<ISyncTransformer> _transformerRegistry;
  private readonly IPluginProviderRegistry<ISyncSink> _sinkRegistry;
  private readonly IPluginProviderRegistry<IHistoryStore> _storeRegistry;
  private readonly PipelineConfig _config;
  private readonly Dictionary<string, IActorRef> _pipelines = [];
  private readonly HashSet<string> _completedPipelines = [];
  private IReadOnlyList<IReadOnlySet<string>> _layers;
  private int _currentLayerIndex = 0;
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  public PipelineManagerActor(
    IPluginProviderRegistry<ISyncSource> sourceRegistry, 
    IPluginProviderRegistry<ISyncTransformer> transformerRegistry, 
    IPluginProviderRegistry<ISyncSink> sinkRegistry,
    IPluginProviderRegistry<IHistoryStore> storeRegistry,
    PipelineConfig config)
  {
    _sourceRegistry = sourceRegistry;
    _transformerRegistry = transformerRegistry;
    _sinkRegistry = sinkRegistry;
    _storeRegistry = storeRegistry;
    _config = config;
    _layers = _config.BuildLayers();
    Receive<StartPipeline>(msg => HandleStartPipeline(msg));
    Receive<PipelineCompleted>(msg => HandlePipelineCompleted(msg));
    Receive<StopPipeline>(msg => HandleStopPipeline(msg));
  }

  protected override void PreStart()
  {
    StartNextLayer();
  }



  private void StartNextLayer()
  {
    if(_currentLayerIndex >= _layers.Count)
    {
      _logger.Info("All pipeline layers have been started.");
      return;
    }
    var layer = _layers[_currentLayerIndex];
    var contexts = _config.Pipelines.ToDictionary(p => p.Name, p => p);
    foreach(var name in layer)
    {
      var context = contexts[name];
      Self.Tell(new StartPipeline(context));
    }
  }

  private void HandleStartPipeline(StartPipeline msg)
  {
    var context = msg.Context;
    if (!_pipelines.ContainsKey(context.Name))
    {
      var source = context.SourceProvider;
      var sourceProvider = _sourceRegistry.GetProvider(source.Type);

      var transformer = context.TransformerProvider;
      var transformerChain = _transformerRegistry.GetProvider(transformer.Type);

      var sink = context.SinkProvider;
      var sinkProvider = _sinkRegistry.GetProvider(sink.Type);

      var store = context.HistoryStoreProvider;
      var storeProvider = _storeRegistry.GetProvider(store?.Type ?? string.Empty);
      
      if(sourceProvider is not null && transformerChain is not null && sinkProvider is not null)
      {
         var pipelineActor = Context.ActorOf(Props.Create(() => new PipelineActor(sourceProvider, transformerChain, sinkProvider, storeProvider, context)), context.Name);
        _pipelines[context.Name] = pipelineActor;
        _logger.Info($"Started pipeline with ID {context.Name}.");
      }
      else
      {
        _logger.Warning($"Failed to create pipeline {context.Name}. Source or Transformer provider not found.");
        return;
      }
    }
    else
    {
      _logger.Warning($"Pipeline with ID {context.Name} already exists.");
    }
    var actor = _pipelines[context.Name];
    actor.Tell(new StartSync(), Sender);
    
  }

  private void HandlePipelineCompleted(PipelineCompleted msg)
  {
    _completedPipelines.Add(msg.Name);
    var currentLayer = _layers[_currentLayerIndex];
    if(_completedPipelines.IsSupersetOf(currentLayer))
    {
      _logger.Info($"All pipelines in layer {_currentLayerIndex} have completed. Starting next layer.");
      _currentLayerIndex++;
      StartNextLayer();
    }
    
    _logger.Info($"Pipeline {msg.Name} completed.");
  }

  private void HandleStopPipeline(StopPipeline msg)
  {
    if(_pipelines.TryGetValue(msg.Name, out var actor))
    {
      actor.Tell(new StopSync(), Sender);
    }
    else
    {
      _logger.Warning($"Pipeline with ID {msg.Name} does not exist.");
    }
  }
}