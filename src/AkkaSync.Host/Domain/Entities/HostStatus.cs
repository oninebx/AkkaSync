using System;

namespace AkkaSync.Host.Domain.Entities;

public enum HostStatus
{
  Syncing,
  Idle,
  Degraded,
  Stopped
}
