using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;
using AkkaSync.Core.Runtime;
using AkkaSync.Core.Runtime.Event;
using AkkaSync.Infrastructure.Abstractions;

namespace AkkaSync.Infrastructure.Actors;

public class PipelineRegistryActor : ReceiveActor
{
  private readonly IPluginProviderRegistry<ISyncSource> _sourceRegistry;
  private readonly IPluginProviderRegistry<ISyncTransform> _transformRegistry;
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
    IPluginProviderRegistry<ISyncTransform> transformRegistry, 
    IPluginProviderRegistry<ISyncSink> sinkRegistry)  
  {
    _actorResolver = actorResolver;
    _sourceRegistry = sourceRegistry;
    _transformRegistry = transformRegistry;
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
      Context.System.EventStream.Publish(new PipelineCompleted(msg.PipelineId));
    });
  }

  protected override void PreStart()
  {
    _schedulerActor = _actorRegistry.Get<PipelineSchedulerActor>();

  }

  private void CreatePipeline(RegistryProtocol.CreatePipeline msg)
  {
    if(_pipelineSpecs == null || !_pipelineSpecs.TryGetValue(msg.Key, out var spec))
    {
      _logger.Warning($"Pipeline spec with name {msg.Key} not found.");

      return;
    }
    var runId = RunId.New();
    var actorName = $"{msg.Key}-${runId}";
    if (Context.Child(actorName).IsNobody())
    {
      var source = spec.Source.Provider ??  throw new NullReferenceException("Source Provider cannot be empty") ;
      var sourceProvider = _sourceRegistry.GetProvider(source);

      var transforms = spec.Plugins.Where(p => p.Type == "transform").Select(p => p.Provider).Distinct().ToList() ?? [];
      var transformerMap = transforms.Select(t => _transformRegistry.GetProvider(t)).Where(p => p is not null).ToDictionary(p => p.Key, p => p);
      var sinkProviderMap = spec.Sinks.Select(sink => _sinkRegistry.GetProvider(sink.Provider)).Where(p => p is not null).Distinct().ToDictionary(p => p.Key, p => p);

      if (sourceProvider is not null && transformerMap.Count == transforms.Count && sinkProviderMap is not null)
      {
        var pipelineId = new PipelineId(runId, msg.Key);
        var pipelineActor = _actorResolver.ActorOf<PipelineActor>(Context, pipelineId.ToString(), sourceProvider, transformerMap, sinkProviderMap, pipelineId, spec.BatchSize, spec.Plugins);
        pipelineActor.Tell(new SharedProtocol.Start());
      }
      else
      {
        _logger.Warning($"Failed to create pipeline {msg.Key}. Source, Transformer or Sink provider not found.");
        _schedulerActor.Tell(new PipelineSkipped(new PipelineId(runId, msg.Key), $"Source, Transformer or Sink provider not found for pipeline {msg.Key}."));
        return;
      }
    }
    else
    {
      _logger.Warning($"Pipeline with ID {msg.Key} already exists.");
    }
  }
}