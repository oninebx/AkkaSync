using System;
using AkkaSync.Core.Application.Diagnosis;
using AkkaSync.Host.Application.Scheduling;
using AkkaSync.Host.Application.Store;
using AkkaSync.Host.Application.Syncing;

namespace AkkaSync.Host.Application.Dashboard;

public interface IDashboardStore
{
  SyncState SyncState { get; }
  PipelineSchedules Schedules { get; }
  DiagnosisJournal Journal { get; }
  
  void Update<TValue>(TValue state) where TValue : IStoreValue;
  IReadOnlyList<IStoreValue> GetEventsToReplay(long lastSeenSequence);
}
