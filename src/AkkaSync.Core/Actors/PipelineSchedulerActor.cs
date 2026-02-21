using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Schedules;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Runtime.PipelineRegistry;
using NCrontab;

namespace AkkaSync.Core.Actors;

public class PipelineSchedulerActor : ReceiveActor
{
  private IReadOnlyDictionary<string, ScheduleSpec>? _schedules;
  private readonly Dictionary<string, ICancelable> _jobs = [];
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private IActorRef? _pipelineRegistry;
  public PipelineSchedulerActor()
  {
    Receive<SchedulerProtocol.Initialize>(msg =>
    {
      _pipelineRegistry = msg.RegistryActor;
      var enabledSchedules = msg.Options.Schedules?.Where(kv => kv.Value.Enabled).Select(kv => kv.Value).ToList()?? [];
      var duplicated = enabledSchedules.GroupBy(s => s.Pipeline).FirstOrDefault(g => g.Count() > 1);
      if (duplicated != null)
      {
        Context.System.EventStream.Publish(new DuplicateScheduleDetected(duplicated.Key));
      }
      var distinctEnabledSchedules = enabledSchedules.GroupBy(s => s.Pipeline).Select(g => g.First()).ToList();
      _schedules = enabledSchedules.ToDictionary(s => s.Pipeline, s => s);
      _logger.Info("{0} actor initialized at {1}.", Self.Path.Name, DateTimeOffset.UtcNow);
      Context.Parent.Tell(new SchedulerInitialized());
    });

    Receive<SharedProtocol.Start>(_ => HandleStart());

    Receive<SchedulerProtocol.Trigger>(msg =>
    {
      _logger.Info($"Pipeline triggered: { msg.Name }");
      _pipelineRegistry.Tell(new RegistryProtocol.CreatePipeline(msg.Name));
      var spec = _schedules!.FirstOrDefault(s => s.Key.Contains(msg.Name)).Value;
      Context.System.EventStream.Publish(new PipelineTriggered(msg.Name));
    });

    Receive<PipelineCompleted>(msg => HandlePipeline(msg.PipelineId));
    Receive<PipelineSkipped>(msg => HandlePipeline(msg.PipelineId));
  }

  private void HandlePipeline(PipelineId id)
  {
    var spec = _schedules![id.Name];
    var nextUtc = ScheduleNextRun(spec);
    Context.System.EventStream.Publish(new PipelineScheduled(id.Name, nextUtc));
  }

  private DateTime ScheduleNextRun(ScheduleSpec spec)
  {
    var schedule = CrontabSchedule.Parse(spec.Cron);
    var nextUtc = schedule.GetNextOccurrence(DateTime.UtcNow);
    var delay = nextUtc - DateTime.UtcNow;

    var cancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(
      delay,
      Self,
      new SchedulerProtocol.Trigger(spec.Pipeline),
      Self
    );

    if(_jobs.TryGetValue(spec.Pipeline, out var job))
    {
      job.Cancel();
    }
    _jobs[spec.Pipeline] = cancelable;
    _logger.Info("{0} will run at {1}. Please wait for {2}.", spec.Pipeline, nextUtc, delay);

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
      var nextUtc = ScheduleNextRun(schedule.Value);
      Context.System.EventStream.Publish(new PipelineScheduled(schedule.Value.Pipeline, nextUtc));
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
