using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Common;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Schedules;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Runtime;
using NCrontab;

namespace AkkaSync.Core.Actors;

public class PipelineSchedulerActor : ReceiveActor
{
  private IReadOnlyDictionary<string, string>? _schedules;
  private readonly Dictionary<string, ICancelable> _jobs = [];
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private IActorRef _pipelineRegistry;
  public PipelineSchedulerActor(ISyncActorRegistry actorRegistry)
  {
    _pipelineRegistry = actorRegistry.Get<PipelineRegistryActor>();

    Receive<SchedulerProtocol.Initialize>(msg =>
    {
      _schedules = msg.Schedules;
      _logger.Info("{0} actor initialized at {1}.", Self.Path.Name, DateTimeOffset.UtcNow);
      Context.Parent.Tell(new SchedulerInitialized());
    });

    Receive<SharedProtocol.Start>(_ => HandleStart());

    Receive<SchedulerProtocol.Trigger>(msg =>
    {
      _logger.Info($"Pipeline triggered: { msg.Id }");
      _pipelineRegistry.Tell(new RegistryProtocol.CreatePipeline(msg.Id));
      var spec = _schedules!.FirstOrDefault(s => s.Key.Contains(msg.Id)).Value;
      Context.System.EventStream.Publish(new PipelineTriggered(msg.Id));
    });

    Receive<PipelineCompleted>(msg => HandlePipeline(msg.PipelineId));
    Receive<PipelineSkipped>(msg => HandlePipeline(msg.PipelineId));
  }

  private void HandlePipeline(PipelineId id)
  {
    var spec = _schedules![id.Pid];
    var nextUtc = ScheduleNextRun(id.Pid, spec);
    Context.System.EventStream.Publish(new PipelineScheduled(id.Pid, nextUtc));
  }

  private DateTime ScheduleNextRun(string pipeline, string cron)
  {
    var schedule = CrontabSchedule.Parse(cron);
    var nextUtc = schedule.GetNextOccurrence(DateTime.UtcNow);
    var delay = nextUtc - DateTime.UtcNow;

    var cancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(
      delay,
      Self,
      new SchedulerProtocol.Trigger(pipeline),
      Self
    );

    if(_jobs.TryGetValue(pipeline, out var job))
    {
      job.Cancel();
    }
    _jobs[pipeline] = cancelable;
    _logger.Info("{0} will run at {1}. Please wait for {2}.", pipeline, nextUtc, delay);

    return nextUtc;
  }

  private void HandleStart()
  {
    if(_schedules == null)
    {
      _logger.Warning("PipelineSchedulerActor received Start message before initialization. Ignoring.");
      return;
    }
    foreach(var schedule in _schedules)
    {
      var nextUtc = ScheduleNextRun(schedule.Key, schedule.Value);
      Context.System.EventStream.Publish(new PipelineScheduled(schedule.Key, nextUtc));
    }
  }

  protected override void PostStop()
  {
    foreach(var job in _jobs.Values)
    {
      job.Cancel();
    }
    _logger.Info("{0} actor stopped at {1}.", Self.Path.Name, DateTimeOffset.UtcNow);
  }
}
