using System;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Domain.Entities;

namespace AkkaSync.Host.Infrastructure.StateStore;

public class InMomeryHostStateStore : IHostStateStore
{
  private volatile HostSnapshot _snapshot = HostSnapshot.Empty;
  public HostSnapshot Snapshot => _snapshot;
  private readonly IHostSnapshotPublisher _publisher;
  private readonly ILogger<InMomeryHostStateStore> _logger;


  public InMomeryHostStateStore(IHostSnapshotPublisher publisher, ILogger<InMomeryHostStateStore> logger)
  {
    _publisher = publisher;
    _logger = logger;
  }

  public void Update(HostSnapshot snapshot)
  {
   _snapshot = snapshot;
   _publisher.BroadcastSnapshot(snapshot);
  _logger.LogInformation("Broadcast a new snapshot at {1}.", DateTimeOffset.UtcNow);

  }
}
