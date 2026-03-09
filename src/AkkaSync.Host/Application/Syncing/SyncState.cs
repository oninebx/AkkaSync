using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Host.Application.Syncing;

public sealed record SyncState(
  HostStatus Status,
  DateTimeOffset StartAt) : IStoreValue
{
  public static SyncState Empty => new(
    Status: HostStatus.Idle,
    StartAt: DateTimeOffset.UtcNow
  );
}
