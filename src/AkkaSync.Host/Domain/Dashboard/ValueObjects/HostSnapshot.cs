using System;
using System.Collections.Immutable;
using AkkaSync.Abstractions;
using AkkaSync.Host.Application.Store;

namespace AkkaSync.Host.Domain.Dashboard.ValueObjects;

public sealed record HostSnapshot(
  HostStatus Status,
  ImmutableList<PipelineSnapshot> Pipelines, 
  DateTimeOffset StartAt) : IStoreValue
{
  public static HostSnapshot Empty => new(
    Status: HostStatus.Idle,
    Pipelines: [],
    StartAt: DateTimeOffset.UtcNow
  );
}
