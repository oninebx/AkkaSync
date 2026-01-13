using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Schedules.Events;

namespace AkkaSync.Host.Application.Scheduling;

public static class ScheduleStateReducer
{
  public static PipelineSchedules Reduce(PipelineSchedules current, ISyncEvent @event) => @event switch
  {
    SchedulerStarted e => current with { Specs = e.Specs },
    PipelineScheduled e => current with { Jobs = [.. current.Jobs, new PipelineJob(e.Name, e.NextUtc)] },
    PipelineTriggered e => current with { Jobs = [.. current.Jobs.Where(j => j.Name != e.Name)]},
    _ => current
  };
}
