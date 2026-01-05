using System;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Application.HostState.Events;

public sealed record SnapshotUpdated : IDashboardEvent
{
  public SnapshotUpdated(HostSnapshot Snapshot)
  {
    Payload = Snapshot;
  }

  public string TypeName => "sync.snapshot.updated";

  public object Payload { get; init; }
}
