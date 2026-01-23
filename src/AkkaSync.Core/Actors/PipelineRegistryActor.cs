using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;
using AkkaSync.Core.Runtime.PipelineManager;

namespace AkkaSync.Core.Actors;

public class PipelineRegistryActor : ReceiveActor
{
  private readonly IPluginProviderRegistry<ISyncSource> _sourceRegistry;
  private readonly IPluginProviderRegistry<ISyncTransformer> _transformerRegistry;
  private readonly IPluginProviderRegistry<ISyncSink> _sinkRegistry;
  private readonly IPluginProviderRegistry<IHistoryStore> _storeRegistry;
  private readonly Dictionary<string, PipelineSpec> _pipelineSpecs;
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private IActorRef? _schedulerActor;
  public PipelineRegistryActor(
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
    _pipelineSpecs = options.Pipelines.ToDictionary(p => p.Name, p => p);

    Receive<PipelineManagerProtocol.CreatePipeline>(msg => CreatePipeline(msg));

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

    Receive<SharedProtocol.RegisterPeer>(msg => {
      _schedulerActor = msg.PeerRef;
      _logger.Info("{0} actor is ready at {1}.", Self.Path.Name, DateTimeOffset.UtcNow);
      var pipelines = _pipelineSpecs.Values.Select(s => new PipelineInfo(s.Name)).ToList().AsReadOnly();
      Context.Parent.Tell(new PeerRegistered(Self.Path.Name, pipelines));
    });
  }

  private void CreatePipeline(PipelineManagerProtocol.CreatePipeline msg)
  {
    if(_pipelineSpecs.TryGetValue(msg.Name, out var spec) is false)
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
}