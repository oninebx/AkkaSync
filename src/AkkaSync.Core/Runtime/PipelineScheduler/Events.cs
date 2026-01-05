using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Runtime.PipelineScheduler;

public sealed record SchedulerStarted(IReadOnlyDictionary<string, IReadOnlyList<string>> Schedules) : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}