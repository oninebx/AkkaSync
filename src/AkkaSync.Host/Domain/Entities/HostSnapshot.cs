using System;
using System.Collections.Immutable;

namespace AkkaSync.Host.Domain.Entities;

public record HostSnapshot(HostStatus Status, ImmutableDictionary<string, PipelineSnapshot> Pipelines, DateTimeOffset StartAt)
{
  public static HostSnapshot Empty => new(
    Status: HostStatus.Idle,
    Pipelines: ImmutableDictionary<string, PipelineSnapshot>.Empty,
    StartAt: DateTimeOffset.UtcNow
  );
}
