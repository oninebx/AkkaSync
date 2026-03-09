using System;

namespace AkkaSync.Abstractions.Models;

public sealed record ScheduleOptions
{
  public IDictionary<string, ScheduleSpec>? Schedules {get; init;}
}
