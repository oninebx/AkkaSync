using System;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Application.Dashboard.Events;

public record SyncEventEmitted(RecentEvent SyncEvent) : IDashboardEvent
{
  public string TypeName => "sync.recentevent.emitted";

  public object Payload => throw new NotImplementedException();
}
