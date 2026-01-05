using System;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Application.Dashboard.Events;

public record SyncEventEmitted : IDashboardEvent
{
  public SyncEventEmitted(RecentEvent syncEvent)
  {
    Payload = syncEvent;
  }
  public string TypeName => "sync.recentevent.emitted";

  public object Payload {get; init;} 
}
