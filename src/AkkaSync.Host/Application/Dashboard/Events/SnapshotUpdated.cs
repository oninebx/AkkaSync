using System;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Application.HostState.Events;

public record SnapshotUpdated(HostSnapshot Snapshot) : IDashboardEvent
{
  public string TypeName => "host.snapshot.updated";

  public object Payload => Snapshot;
}
