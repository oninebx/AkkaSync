using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Application.Diagnosis;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Core.Domain.Workers.Events;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Application.Store;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Application.Dashboard;

public static class DashboardEventMapper
{
  public static DashboardEvent TryMap(IStoreValue value, ISyncEvent? @event = null)
  {
    return value switch
    {
      HostSnapshot snapshot => new DashboardEvent("sync.snapshot.updated", snapshot),
      PipelineSchedules schedules => @event switch {
        SchedulerStarted e => new DashboardEvent("scheduler.specs.initialized", e.Specs),
        PipelineScheduled e => new DashboardEvent("scheduler.jobs.added", schedules.Jobs.FirstOrDefault(j => j.Name == e.Name)!),
        PipelineTriggered e => new DashboardEvent("scheduler.jobs.removed", e.Name),
        _ => new DashboardEvent("scheduler.none", schedules)
      },
      DiagnosisJournal journal => @event switch {
        WorkerFailed e => new DashboardEvent("diagnosis.errors.added", journal.Records.LastOrDefault()!),
        _ => new DashboardEvent("diagnosis.initialized", journal)
      },
      _ => throw new NotImplementedException(nameof(value))
    };
  }
}
