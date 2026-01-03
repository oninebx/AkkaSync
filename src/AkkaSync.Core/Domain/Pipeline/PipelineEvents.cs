using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Core.Domain.Pipeline;

public sealed record PipelineStarted(PipelineId PipelineId, DateTimeOffset At) : ISyncEvent
{
  public DateTimeOffset Timestamp => At;
}

public sealed record PipelineCompleted(PipelineId PipelineId) : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}