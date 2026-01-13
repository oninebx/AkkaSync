using System;

namespace AkkaSync.Host.Application.Syncing;

public enum HostStatus
{
  Syncing,
  Idle,
  Degraded,
  Stopped
}
