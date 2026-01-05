using System;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Domain.Dashboard.Repositories;

public interface IDashboardStore
{
  HostSnapshot Snapshot { get; }
  PipelineSchedules Schedules { get; }
  void Update<TValue>(TValue state) where TValue : IStoreValue;
  IReadOnlyList<IStoreValue> GetEventsToReplay(long lastSeenSequence);
}
