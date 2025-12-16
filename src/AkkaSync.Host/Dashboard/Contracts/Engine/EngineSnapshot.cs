using System;

namespace AkkaSync.Host.Dashboard.Contracts.Engine;

public sealed record EngineSnapshot(EngineStatus Status)
{
  public static EngineSnapshot Empty => new(
    Status: EngineStatus.Stopped
  );
}
