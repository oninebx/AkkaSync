using System;
using AkkaSync.Host.Dashboard.Contracts.Engine;

namespace AkkaSync.Host.Domain.Entities;

public record HostSnapshot(HostStatus Status, DateTimeOffset Timestamp)
{
  public static HostSnapshot Empty => new HostSnapshot(
    Status: HostStatus.Offline,
    Timestamp: DateTimeOffset.UtcNow
  );
}
