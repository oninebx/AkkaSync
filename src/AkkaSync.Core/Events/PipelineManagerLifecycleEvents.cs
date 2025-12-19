using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Events;

public sealed record PipelineManagerStarted(int PipelinesTotal) : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}

public sealed record PipelineManagerFailed() : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}