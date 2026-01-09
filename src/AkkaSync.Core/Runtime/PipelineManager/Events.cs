using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Runtime.PipelineManager;

public sealed record PipelineManagerStarted(IReadOnlyList<PipelineInfo> Pipelines) : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}

public sealed record PipelineManagerFailed() : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}

// public sealed record PipelineCreationFailed(string Name) : ISyncEvent
// {
//   public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
// }
