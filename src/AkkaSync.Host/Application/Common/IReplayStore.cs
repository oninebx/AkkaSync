using System;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Application.Common;

public interface IReplayStore<T> where T : IStoreValue
{
  IReadOnlyList<T> GetEventsToReplay(long lastSeenSequence);
}
