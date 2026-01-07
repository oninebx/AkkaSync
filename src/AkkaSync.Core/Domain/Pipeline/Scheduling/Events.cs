using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Pipeline.Scheduling;

public sealed record SchedulerStarted(IReadOnlyDictionary<string, string> Specs) : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}

public sealed record PipelineScheduled(string Name, DateTime NextUtc) : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}

public sealed record PipelineTriggered(string Name) : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}