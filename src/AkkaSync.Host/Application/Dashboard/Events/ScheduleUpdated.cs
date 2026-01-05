using System;
using AkkaSync.Host.Application.Messaging;

namespace AkkaSync.Host.Application.Dashboard.Events;

public record ScheduleUpdated : IDashboardEvent
{
  public ScheduleUpdated(IReadOnlyDictionary<string, IReadOnlyList<string>> schedules)
  {
    Payload = schedules;
  }
  public string TypeName => "sync.scheduler.updated";

  public object Payload { get; init; }
}
