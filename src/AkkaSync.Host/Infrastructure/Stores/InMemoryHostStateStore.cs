using System;
using AkkaSync.Host.Application.Common;
using AkkaSync.Host.Application.HostState;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Domain.Dashboard.Repositories;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Infrastructure.Stores;

public class InMemoryHostStateStore( ILogger<InMemoryHostStateStore> logger) : IHostStateStore, IReplayStore<HostSnapshot>
{
  private volatile HostSnapshot _snapshot = HostSnapshot.Empty;
  public HostSnapshot Snapshot => _snapshot;
  private readonly ILogger<InMemoryHostStateStore> _logger = logger;

  public void Update(HostSnapshot snapshot)
  {
   _snapshot = snapshot;
  }

  IReadOnlyList<HostSnapshot> IReplayStore<HostSnapshot>.GetEventsToReplay(long lastSeenSequence)
  {
    return [_snapshot];
  }
}
