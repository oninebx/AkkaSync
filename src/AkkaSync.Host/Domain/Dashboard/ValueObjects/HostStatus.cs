using System;

namespace AkkaSync.Host.Domain.Dashboard.ValueObjects;

public enum HostStatus
{
  Syncing,
  Idle,
  Degraded,
  Stopped
}
