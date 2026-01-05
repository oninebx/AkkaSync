using System;
using Akka.Actor;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Runtime.PipelineScheduler;

namespace AkkaSync.Core.Actors;

public class PipelineSchedulerActor : ReceiveActor
{
  private readonly IReadOnlyDictionary<string, ScheduleSpec> _schedules;
  public PipelineSchedulerActor(ScheduleOptions options)
  {
    _schedules = options.Schedules?.Where(s => s.Value.Enabled).ToDictionary() ?? [];
  }

  protected override void PreStart()
  {
    var schedulesToPublish = _schedules.Values
        .GroupBy(s => s.PipelineId)
        .ToDictionary(
            g => g.Key,
            g => (IReadOnlyList<string>)g.Select(g => g.Cron).ToList()
        ).AsReadOnly();
    Context.System.EventStream.Publish(new SchedulerStarted(schedulesToPublish));
  }
}
