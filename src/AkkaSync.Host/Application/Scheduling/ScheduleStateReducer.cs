using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;

namespace AkkaSync.Host.Application.Scheduling;

public static class ScheduleStateReducer
{
  public static PipelineSchedules Reduce(PipelineSchedules current, INotificationEvent @event) => @event switch
  {
    SyncEngineReady e => current with { Specs = e.Schedules },
    PipelineScheduled e => current with { Jobs = [.. current.Jobs, new PipelineJob(e.Name, e.NextUtc)] },
    PipelineTriggered e => current with { Jobs = [.. current.Jobs.Where(j => j.Name != e.Name)]},
    _ => current
  };
}
