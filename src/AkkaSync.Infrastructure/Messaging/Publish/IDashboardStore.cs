using System;

namespace AkkaSync.Infrastructure.Messaging.Publish;

public interface IDashboardStore
{ 
  void Update<TValue>(TValue state) where TValue : IStoreValue;
  IReadOnlyList<IStoreValue> GetEventsToReplay(long lastSeenSequence);
}
