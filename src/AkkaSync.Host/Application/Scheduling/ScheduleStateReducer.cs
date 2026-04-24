using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Host.Application.Scheduling; 

public static class ScheduleStateReducer
{
  public static ScheduleState Reduce(ScheduleState current, IProjectionEvent @event) => @event switch
  {
    SyncEngineReady e => current with { Specs = e.Schedules.ToDictionary(s => s.Key, s => s.Value.Cron) },
    PipelineScheduled e => current with { Jobs = [.. current.Jobs, new PipelineJob(e.Name, e.NextUtc)] },
    PipelineTriggered e => current with { Jobs = [.. current.Jobs.Where(j => j.Id != e.Pid)]},
    _ => current
  };
}
