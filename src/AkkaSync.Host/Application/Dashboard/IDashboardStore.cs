using System;
using AkkaSync.Core.Application.Diagnosis;
using AkkaSync.Host.Application.Store;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Application.Dashboard;

public interface IDashboardStore
{
  HostSnapshot Snapshot { get; }
  PipelineSchedules Schedules { get; }
  DiagnosisJournal Journal { get; }
  
  void Update<TValue>(TValue state) where TValue : IStoreValue;
  IReadOnlyList<IStoreValue> GetEventsToReplay(long lastSeenSequence);
}
