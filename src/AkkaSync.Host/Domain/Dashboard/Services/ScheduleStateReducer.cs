using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Domain.Dashboard.Services;

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
