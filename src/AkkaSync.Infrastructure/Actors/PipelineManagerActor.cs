using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Schedules;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Domain.Shared.Events;
using AkkaSync.Core.Runtime;
using AkkaSync.Core.Runtime.Event;
using AkkaSync.Infrastructure.Abstractions;

namespace AkkaSync.Infrastructure.Actors;

public class PipelineManagerActor : ReceiveActor
{
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private IActorRef _schedulerActor;
  private IActorRef _registryActor;
  private IReadOnlyList<PipelineSpec> _pipelines = [];
  private IReadOnlyDictionary<string, ScheduleSpec> _schedules = new Dictionary<string, ScheduleSpec>();
  private readonly IPipelineStorage _pipelineStorage;
  private bool _registryReady;
  private bool _schedulerReady;
  private readonly ISyncActorRegistry _actorRegistry;
  private readonly ISyncActorResolver _actorResolver;

  public PipelineManagerActor(ISyncActorRegistry actorRegistry, ISyncActorResolver actorResolver, IPipelineStorage pipelineStorage)
  {
    _actorRegistry = actorRegistry;
    _actorResolver = actorResolver;
    _pipelineStorage = pipelineStorage;

    _registryActor = actorResolver.ActorOf<PipelineRegistryActor>(Context, "pipeline-registry");
    _actorRegistry.Register<PipelineRegistryActor>(_registryActor);

    _schedulerActor = actorResolver.ActorOf<PipelineSchedulerActor>(Context, "pipeline-scheduler");
    _actorRegistry.Register<PipelineSchedulerActor>(_schedulerActor);

    ReceiveAsync<SharedProtocol.Start>(_ => HandleStartAsync());
    Receive<RegistryInitialized>(_ =>
    {
      _logger.Info("PipelineRegistryActor is ready at {0}.", DateTimeOffset.UtcNow);
      _registryReady = true;
      CheckReady();
    });
    Receive<SchedulerInitialized>(_ =>
    {
      _logger.Info("PipelineSchedulerActor is ready at {0}.", DateTimeOffset.UtcNow);
      _schedulerReady = true;
      CheckReady();
    });
  }

  override protected void PreStart()
  {
    Self.Tell(new SharedProtocol.Start());
  }

  private void CheckReady()
  {
    if(_registryReady && _schedulerReady)
    {
      Context.System.EventStream.Publish(new SyncEngineReady(_pipelines, _schedules));
      _schedulerActor!.Tell(new SharedProtocol.Start());
    }
  }

  protected override void PostStop()
  {
    Context.System.EventStream.Publish(new SyncEngineStopped());
    
  }

  private async Task HandleStartAsync()
  {
    _logger.Info("PipelineManagerActor started at {0}", DateTimeOffset.UtcNow);
    var pipelines  = await _pipelineStorage.LoadPipelineSpecificationsAsync();
    var pipelineSpecs = pipelines.Where(p => p.Value.IsActive).ToDictionary(p => p.Key, p => p.Value);
    if(pipelineSpecs.Count == 0)
    {
      _logger.Warning("No pipeline specs found in storage.");
      return;
    }
    _pipelines = [.. pipelineSpecs.Values];

   var pipelineSchedules = pipelineSpecs.Values.ToDictionary(p => p.Name, p => (IReadOnlySet<string>)p.Schedules.ToHashSet());

    _registryActor.Tell(new RegistryProtocol.Initialize(pipelineSpecs));

    _schedulerActor.Tell(new SchedulerProtocol.Initialize(pipelineSchedules));
  }
}
