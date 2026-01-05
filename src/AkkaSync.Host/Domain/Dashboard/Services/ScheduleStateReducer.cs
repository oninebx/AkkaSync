using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Runtime.PipelineScheduler;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Domain.Dashboard.Services;

public static class ScheduleStateReducer
{
  public static PipelineSchedules Reduce(PipelineSchedules current, ISyncEvent @event) => @event switch
  {
    SchedulerStarted e => current with { Data = e.Schedules },
    _ => current
  };
}
