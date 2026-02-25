using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Application.Scheduling;
using AkkaSync.Host.Application.Syncing;
using AkkaSync.Host.Application.Diagnosing;
using AkkaSync.Host.Application.Swapping;
using AkkaSync.Infrastructure.Messaging;

namespace AkkaSync.Host.Application.Dashboard;

public static class DashboardEventMapper
{
  public static DashboardEvent? TryMap(IStoreValue value, INotificationEvent? @event = null)
  {

    return value switch
    {
      SyncState syncState => FromSyncState(syncState, @event),
      PipelineSchedules schedules => FromPipelineSchedules(schedules, @event),
      DiagnosisJournal journal => FromDiagnosisJournal(journal, @event),
      RuntimePluginSet pluginSet => FromComposingPlugins(pluginSet, @event),
      _ => throw new NotImplementedException(nameof(value))
    };
  }

  private static DashboardEvent? FromSyncState(SyncState state, INotificationEvent? @event) => @event switch
  {
    PipelineStartReported e => new DashboardEvent("syncing.pipeline.started", new { Id = e.PipelineId.Name, StartedAt = @event.OccurredAt }),
    PipelineCompleteReported e => new DashboardEvent("syncing.pipeline.completed", new { Id = e.PipelineId.Name, FinishAt = @event.OccurredAt }),
    DashboardInitialized => new DashboardEvent("syncing.state.initialized", state),
    _ => null
  };

  private static DashboardEvent? FromPipelineSchedules(PipelineSchedules schedules, INotificationEvent? @event) => @event switch
  {
    SyncEngineReady e => new DashboardEvent("scheduler.specs.initialized", e.Schedules),
    PipelineScheduled e => new DashboardEvent("scheduler.jobs.added", schedules.Jobs.FirstOrDefault(j => j.Name == e.Name)!),
    PipelineTriggered e => new DashboardEvent("scheduler.jobs.removed", e.Name),
    DashboardInitialized => new DashboardEvent("scheduler.none", schedules),
    _ => null
  };

  private static DashboardEvent? FromDiagnosisJournal(DiagnosisJournal journal, INotificationEvent? @event) => @event switch
  {
    WorkerFailureReported
    or WorkerStartReported
    or WorkerCompleteReported
    or WorkerNonCreationReported
    or PipelineStartReported
    or PipelineSkipReported
    or PipelineCompleteReported => new DashboardEvent("diagnosis.records.added", journal.Records.LastOrDefault()!),
    DashboardInitialized => new DashboardEvent("diagnosis.records.initialized", journal),
    _ => null
  };

  private static DashboardEvent? FromComposingPlugins(RuntimePluginSet pluginSet, INotificationEvent? @event) => @event switch 
  {
    PluginAdded => new DashboardEvent("pluginhub.entries.added", pluginSet.Entries.LastOrDefault()!),
    PluginRemoved e => new DashboardEvent("pluginhub.entries.removed", e.Name),
    PluginUpdated e => new DashboardEvent("pluginhub.entries.updated", new { e.Name, e.Version }),
    DashboardInitialized => new DashboardEvent("pluginhub.entries.initialized", pluginSet),
    _ => null
  };
}
