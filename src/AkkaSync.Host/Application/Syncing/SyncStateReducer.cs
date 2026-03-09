using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Host.Application.Syncing;

public static class SyncStateReducer
{
  public static SyncState Reduce(SyncState current, INotificationEvent @event)
  {
    return @event switch
    {
      SyncEngineReady e => current with { StartAt = @event.OccurredAt },
      _ => current
    };
  }
}
