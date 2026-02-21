using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;
using AkkaSync.Core.Runtime.PipelineManager;
using AkkaSync.Core.Runtime.PipelineRegistry;

namespace AkkaSync.Core.Actors;

public class PipelineRegistryActor : ReceiveActor
{
  private readonly IPluginProviderRegistry<ISyncSource> _sourceRegistry;
  private readonly IPluginProviderRegistry<ISyncTransformer> _transformerRegistry;
  private readonly IPluginProviderRegistry<ISyncSink> _sinkRegistry;
  private readonly IPluginProviderRegistry<IHistoryStore> _storeRegistry;
  private IDictionary<string, PipelineSpec>? _pipelineSpecs;
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private IActorRef? _schedulerActor;
  public PipelineRegistryActor(
    IPluginProviderRegistry<ISyncSource> sourceRegistry, 
    IPluginProviderRegistry<ISyncTransformer> transformerRegistry, 
    IPluginProviderRegistry<ISyncSink> sinkRegistry,
    IPluginProviderRegistry<IHistoryStore> storeRegistry)  
  {
    _sourceRegistry = sourceRegistry;
    _transformerRegistry = transformerRegistry;
    _sinkRegistry = sinkRegistry;
    _storeRegistry = storeRegistry;
    
    Receive<RegistryProtocol.Initialize>(msg => {
      _logger.Info("{0} actor started at {1}.", Self.Path.Name, DateTimeOffset.UtcNow);
      _schedulerActor = msg.SchedulerActor;
      _pipelineSpecs = msg.Options.Pipelines;
      Context.Parent.Tell(new RegistryInitialized());
    });
    Receive<RegistryProtocol.CreatePipeline>(msg => CreatePipeline(msg));

    Receive<PipelineSkipped>(msg => {
      _logger.Info($"Pipeline {msg.PipelineId} skipped (no workers created).");
      _schedulerActor.Tell(msg);
     Context.System.EventStream.Publish(new PipelineSkipReported(msg.PipelineId, msg.Reason));
    });
    Receive<PipelineCompleted>(msg => {
      _logger.Info($"Pipeline {msg.PipelineId} completed");
      _schedulerActor.Tell(msg);
      Context.System.EventStream.Publish(new PipelineCompleteReported(msg.PipelineId));
    });
  }

  private void CreatePipeline(RegistryProtocol.CreatePipeline msg)
  {
    if(_pipelineSpecs == null || !_pipelineSpecs.TryGetValue(msg.Name, out var spec))
    {
      _logger.Warning($"Pipeline spec with name {msg.Name} not found.");

      return;
    }
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
        pipelineActor.Tell(new SharedProtocol.Start());
      }
      else
      {
        _logger.Warning($"Failed to create pipeline {msg.Name}. Source, Transformer or Sink provider not found.");
        _schedulerActor.Tell(new PipelineSkipped(new PipelineId(runId, msg.Name), $"Source, Transformer or Sink provider not found for pipeline {msg.Name}."));
        return;
      }
    }
    else
    {
      _logger.Warning($"Pipeline with ID {msg.Name} already exists.");
    }
  }
}