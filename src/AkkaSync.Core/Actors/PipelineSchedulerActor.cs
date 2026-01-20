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
  private IActorRef? _pipelineRegistry;
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
        var nextUtc = ScheduleNextRun(schedule.Value);
        Context.System.EventStream.Publish(new PipelineScheduled(schedule.Value.Pipeline, nextUtc));
      }
    });

    Receive<PipelineSchedulerProtocol.Trigger>(msg =>
    {
      _logger.Info($"Pipeline triggered: { msg.Name }");
      _pipelineRegistry.Tell(new PipelineManagerProtocol.CreatePipeline(msg.Name));
      var spec = _schedules.FirstOrDefault(s => s.Key.Contains(msg.Name)).Value;
      Context.System.EventStream.Publish(new PipelineTriggered(msg.Name));
    });

    Receive<PipelineCompleted>(msg =>
    {
      var spec = _schedules[msg.PipelineId.Name];
      var nextUtc = ScheduleNextRun(spec);
      Context.System.EventStream.Publish(new PipelineScheduled(msg.PipelineId.Name, nextUtc));
    });

    Receive<SharedProtocol.RegisterPeer>(msg => {
      _pipelineRegistry = msg.PeerRef;
      _logger.Info("{0} actor is ready at {1}.", Self.Path.Name, DateTimeOffset.UtcNow);
      var schedulesToPublish = _schedules.ToDictionary(s => s.Key, s => s.Value.Cron).AsReadOnly();
      Context.Parent.Tell(new PeerRegistered(Self.Path.Name, schedulesToPublish));
    });
  }

  private DateTime ScheduleNextRun(ScheduleSpec spec)
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

    return nextUtc;
  }
}
