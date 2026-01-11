using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Schedules;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Runtime.PipelineManager;
using NCrontab;

namespace AkkaSync.Core.Actors;

public class PipelineSchedulerActor : ReceiveActor
{
  private readonly IReadOnlyDictionary<string, ScheduleSpec> _schedules;
  private readonly Dictionary<string, ICancelable> _jobs = [];
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  public PipelineSchedulerActor(ScheduleOptions options)
  {

    var enabledSchedules = options.Schedules?.Where(kv => kv.Value.Enabled).Select(kv => kv.Value).ToList()?? [];
    var duplicated = enabledSchedules.GroupBy(s => s.Pipeline).FirstOrDefault(g => g.Count() > 1);
    if (duplicated != null)
    {
      throw new InvalidOperationException($"Multiple enabled schedules found for pipeline '{duplicated.Key}'.");
    }

    _schedules = enabledSchedules.ToDictionary(s => s.Pipeline, s => s);

    Receive<PipelineSchedulerProtocol.Start>(_ =>
    {
      foreach(var schedule in _schedules)
      {
        ScheduleNextRun(schedule.Value);
      }
    });

    Receive<PipelineSchedulerProtocol.Trigger>(msg =>
    {
      _logger.Info($"Pipeline triggered: { msg.Name }");
      var manager = Context.ActorSelection("/user/sync-runtime/pipeline-manager");
      manager.Tell(new PipelineManagerProtocol.CreatePipeline(msg.Name));
      var spec = _schedules.FirstOrDefault(s => s.Key.Contains(msg.Name)).Value;
      Context.System.EventStream.Publish(new PipelineTriggered(msg.Name));
    });

    Receive<PipelineCompleted>(msg =>
    {
      var spec = _schedules[msg.PipelineId.Name];
      ScheduleNextRun(spec);
    });
  }
  protected override void PreStart()
  {
    Context.System.EventStream.Subscribe(Self, typeof(PipelineCompleted));
    var schedulesToPublish = _schedules.ToDictionary(s => s.Key, s => s.Value.Cron).AsReadOnly();
    Context.System.EventStream.Publish(new SchedulerStarted(schedulesToPublish));
  }

  protected override void PostStop()
  {
    Context.System.EventStream.Unsubscribe(Self, typeof(PipelineCompleted));
  }

  private void ScheduleNextRun(ScheduleSpec spec)
  {
    var schedule = CrontabSchedule.Parse(spec.Cron);
    var nextUtc = schedule.GetNextOccurrence(DateTime.UtcNow);
    var delay = nextUtc - DateTime.UtcNow;

    var cancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(
      delay,
      Self,
      new PipelineSchedulerProtocol.Trigger(spec.Pipeline),
      Self
    );

    if(_jobs.TryGetValue(spec.Pipeline, out var job))
    {
      job.Cancel();
    }
    _jobs[spec.Pipeline] = cancelable;
    _logger.Info("{0} will run at {1}. Please wait for {2}.", spec.Pipeline, nextUtc, delay);

    Context.System.EventStream.Publish(new PipelineScheduled(spec.Pipeline, nextUtc));
  }
}
