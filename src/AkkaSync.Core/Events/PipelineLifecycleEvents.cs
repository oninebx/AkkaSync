using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Events;

public sealed record PipelineStarted(string Id) : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}