using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Common;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;
using AkkaSync.Core.Runtime;
using AkkaSync.Core.Runtime.Event;

namespace AkkaSync.Core.Actors;

public class PipelineRegistryActor : ReceiveActor
{
  private readonly IPluginProviderRegistry<ISyncSource> _sourceRegistry;
  private readonly IPluginProviderRegistry<ISyncTransformer> _transformerRegistry;
  private readonly IPluginProviderRegistry<ISyncSink> _sinkRegistry;
  private IDictionary<string, PipelineSpec>? _pipelineSpecs;
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private IActorRef? _schedulerActor;
  private readonly ISyncActorRegistry _actorRegistry;
  private readonly ISyncActorResolver _actorResolver;
  public PipelineRegistryActor(
    ISyncActorRegistry actorRegistry,
    ISyncActorResolver actorResolver,
    IPluginProviderRegistry<ISyncSource> sourceRegistry, 
    IPluginProviderRegistry<ISyncTransformer> transformerRegistry, 
    IPluginProviderRegistry<ISyncSink> sinkRegistry)  
  {
    _actorResolver = actorResolver;
    _sourceRegistry = sourceRegistry;
    _transformerRegistry = transformerRegistry;
    _sinkRegistry = sinkRegistry;
    _actorRegistry = actorRegistry;

    Receive<RegistryProtocol.Initialize>(msg => {
      _logger.Info("{0} actor started at {1}.", Self.Path.Name, DateTimeOffset.UtcNow);
      _pipelineSpecs = msg.Pipelines;
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

  protected override void PreStart()
  {
    _schedulerActor = _actorRegistry.Get<PipelineSchedulerActor>();

  }

  private void CreatePipeline(RegistryProtocol.CreatePipeline msg)
  {
    if(_pipelineSpecs == null || !_pipelineSpecs.TryGetValue(msg.Id, out var spec))
    {
      _logger.Warning($"Pipeline spec with name {msg.Id} not found.");

      return;
    }
    var runId = RunId.New();
    var actorName = $"{msg.Id}-${runId}";
    if (Context.Child(actorName).IsNobody())
    {
      var source = spec.SourceProvider;
      var sourceProvider = _sourceRegistry.GetProvider(source.Type);

      var transformer = spec.TransformerProvider;
      var transformerChain = _transformerRegistry.GetProvider(transformer.Type);

      var sink = spec.SinkProvider;
      var sinkProvider = _sinkRegistry.GetProvider(sink.Type);

      if (sourceProvider is not null && transformerChain is not null && sinkProvider is not null)
      {
        var pipelineId = new PipelineId(runId, msg.Id);
        var pipelineActor = _actorResolver.ActorOf<PipelineActor>(Context, pipelineId.ToString(), sourceProvider, transformerChain, sinkProvider, pipelineId, spec);
        pipelineActor.Tell(new SharedProtocol.Start());
      }
      else
      {
        _logger.Warning($"Failed to create pipeline {msg.Id}. Source, Transformer or Sink provider not found.");
        _schedulerActor.Tell(new PipelineSkipped(new PipelineId(runId, msg.Id), $"Source, Transformer or Sink provider not found for pipeline {msg.Id}."));
        return;
      }
    }
    else
    {
      _logger.Warning($"Pipeline with ID {msg.Id} already exists.");
    }
  }
}