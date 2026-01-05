using System;
using AkkaSync.Host.Application.Dashboard.Events;
using AkkaSync.Host.Application.HostState.Events;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Application.Messaging;

public static class DashboardEventMapper
{
  public static IDashboardEvent TryMap(IStoreValue value)
  {
    return value switch
    {
      HostSnapshot snapshot => new SnapshotUpdated(snapshot),
      PipelineSchedules schedules => new ScheduleUpdated(schedules.Data),
      _ => throw new NotImplementedException(nameof(value))
    };
  }
}
