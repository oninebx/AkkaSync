using System;

namespace AkkaSync.Host.Infrastructure.SignalR;

public sealed class DashboardClientState
{
  public string ConnectionId { get; }
  public long LastSeenSequence { get; set; }

  public DashboardClientState(string connectionId, long lastSeenSequence)
  {
    ConnectionId = connectionId;
    LastSeenSequence = lastSeenSequence;
  }
}
