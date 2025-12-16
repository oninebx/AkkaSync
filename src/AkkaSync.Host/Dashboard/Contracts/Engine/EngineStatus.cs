using System;

namespace AkkaSync.Host.Dashboard.Contracts.Engine;

public enum EngineStatus
{
  Syncing,
  Idle,
  Paused,
  Stopped,
  Failed
}
