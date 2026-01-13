using System;
using System.Collections.Immutable;
using AkkaSync.Abstractions;
using AkkaSync.Host.Application.Store;

namespace AkkaSync.Host.Application.Syncing;

public sealed record SyncState(
  HostStatus Status,
  ImmutableList<PipelineSnapshot> Pipelines, 
  DateTimeOffset StartAt) : IStoreValue
{
  public static SyncState Empty => new(
    Status: HostStatus.Idle,
    Pipelines: [],
    StartAt: DateTimeOffset.UtcNow
  );
}
