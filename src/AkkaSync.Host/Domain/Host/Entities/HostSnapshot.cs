using System;
using AkkaSync.Host.Domain.Entities;

namespace AkkaSync.Host.Domain.Host;

public record HostSnapshot(HostStatus Status, DateTimeOffset Timestamp)
{
  public static HostSnapshot Empty => new HostSnapshot(
    Status: HostStatus.Offline,
    Timestamp: DateTimeOffset.UtcNow
  );
}
