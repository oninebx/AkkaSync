using System;
using System.Collections.Immutable;

namespace AkkaSync.Host.Domain.Entities;

public record HostSnapshot(
  HostStatus Status,
  int PipelinesTotal,
  ImmutableList<PipelineSnapshot> Pipelines, 
  DateTimeOffset StartAt)
{
  public static HostSnapshot Empty => new(
    Status: HostStatus.Idle,
    PipelinesTotal: 0,
    Pipelines: [],
    StartAt: DateTimeOffset.UtcNow
  );
}
