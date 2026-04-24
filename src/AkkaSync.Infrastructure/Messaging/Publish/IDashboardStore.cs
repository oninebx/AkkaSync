using AkkaSync.Abstractions;
using System;

namespace AkkaSync.Infrastructure.Messaging.Publish;

public interface IDashboardStore
{ 
  void Update<TValue>(TValue state) where TValue : IStateSnashot;
  IReadOnlyList<IStateSnashot> GetEventsToReplay(long lastSeenSequence);
}
