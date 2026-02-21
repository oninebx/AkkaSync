using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Schedules;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;
using AkkaSync.Core.Runtime.PipelineManager;
using AkkaSync.Core.Runtime.PipelineRegistry;

namespace AkkaSync.Core.Actors;

public class PipelineManagerActor : ReceiveActor
{
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private IActorRef? _schedulerActor;
  private IActorRef? _registryActor;
  private IDictionary<string, Props> _props;
  private IReadOnlyList<PipelineInfo> _pipelines = [];
  private IReadOnlyDictionary<string, string> _schedules = new Dictionary<string, string>();
  private readonly IPipelineStorage _pipelineStorage;
  private bool _registryReady;
  private bool _schedulerReady;
  public PipelineManagerActor(IDictionary<string, Props> props, IPipelineStorage pipelineStorage)
  {
    _props = props;
    _pipelineStorage = pipelineStorage;

    ReceiveAsync<SharedProtocol.Start>(_ => HandleStartAsync());
    Receive<RegistryInitialized>(_ => {
      _logger.Info("PipelineRegistryActor is ready at {0}.", DateTimeOffset.UtcNow);
      _registryReady = true;
      CheckReady();
    });
    Receive<SchedulerInitialized>(_ => {
      _logger.Info("PipelineSchedulerActor is ready at {0}.", DateTimeOffset.UtcNow);
      _schedulerReady = true;
      CheckReady();
    });
  }

  override protected void PreStart()
  {
    var strategy = new OneForOneStrategy(
      maxNrOfRetries: 3,
      withinTimeRange: TimeSpan.FromSeconds(10),
      localOnlyDecider: ex =>
      {
        return Directive.Restart;
      }
    );
    _schedulerActor = Context.ActorOf(_props["pipeline-scheduler"].WithSupervisorStrategy(strategy), "pipeline-scheduler");
    _registryActor = Context.ActorOf(_props["pipeline-registry"].WithSupervisorStrategy(strategy), "pipeline-registry");

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
    var (pipelineOptions, scheduleOptions) = await _pipelineStorage.LoadSpecificationsAsync();
    var pipelineSpecs = pipelineOptions.Pipelines?.Where(p => p.Value.IsActive).ToDictionary(p => p.Key, p => p.Value);
    if(pipelineSpecs?.Any() != true)
    {
      _logger.Warning("No pipeline specs found in storage.");
      return;
    }
    var enabledSchedules = scheduleOptions.Schedules?.Where(s => s.Value.Enabled).Select(s => s.Value).ToList();

    _pipelines = pipelineSpecs.Select(s => new PipelineInfo(s.Key)).ToList().AsReadOnly();
    _schedules = enabledSchedules?.ToDictionary(s => s.Pipeline, s => s.Cron) ?? [];
    _registryActor.Tell(new RegistryProtocol.Initialize(_schedulerActor!, pipelineOptions));
    _schedulerActor.Tell(new SchedulerProtocol.Initialize(_registryActor!, scheduleOptions));
  }
}
