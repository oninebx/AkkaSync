using System;

namespace AkkaSync.Abstractions.Models;

public sealed record ScheduleOptions
{
  public IReadOnlyDictionary<string, ScheduleSpec>? Schedules {get; init;}
}
