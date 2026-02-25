using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Runtime.Event;

public sealed record PipelinManagerStarted();
public sealed record PipelineManagerFailed()
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}

public sealed record RegistryInitialized();

// public sealed record PipelineCreationFailed(string Name) : ISyncEvent
// {
//   public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
// }
