using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;
using AkkaSync.Host.Application.Scheduling;
using AkkaSync.Host.Application.Syncing;
using AkkaSync.Host.Application.Diagnosing;
using AkkaSync.Host.Application.Swapping;
using AkkaSync.Infrastructure.Messaging.Contract.Swap;
using AkkaSync.Infrastructure.Messaging.Publish;
using AkkaSync.Infrastructure.Messaging.Contract.Update;
using AkkaSync.Host.Application.Pipeline;
using AkkaSync.Host.Application.Dashboard.NotificationMappings;

namespace AkkaSync.Host.Application.Dashboard;

public class DashboardEventMapper: IEventNotificationMapper
{
  private readonly IEnumerable<IEventNotificationMapping> _mappings;
  public DashboardEventMapper(IEnumerable<IEventNotificationMapping> mappings)
  {
    _mappings = mappings;
  }

  public EventNotification? TryMap(IStoreValue value, INotificationEvent? @event = null)
  {
    foreach (var mapping in _mappings)
    {
      if (!mapping.CanHandle(value))
      {
        continue;
      }
      return mapping.TryMap(value, @event);
    }
    return null;
  }

  //public EventNotification? TryMap(IStoreValue value, INotificationEvent? @event = null)
  //{

  //  return value switch
  //  {
  //    PipelineState pipelineState => FromPipelineState(pipelineState, @event),
  //    SyncState syncState => FromSyncState(syncState, @event),
  //    PipelineSchedules schedules => FromPipelineSchedules(schedules, @event),
  //    DiagnosisJournal journal => FromDiagnosisJournal(journal, @event),
  //    RuntimePluginSet pluginSet => FromSwappingPlugins(pluginSet, @event),
  //    _ => throw new NotImplementedException(nameof(value))
  //  };
  //}

  //private static EventNotification? FromPipelineState(PipelineState state, INotificationEvent? @event) => @event switch
  //{
  //  SyncEngineReady e => new EventNotification("pipeline.specs.initialized", e.Pipelines),
  //  DashboardInitialized => new EventNotification("pipeline.specs.initialized", state.Pipelines),
  //  _ => null
  //};

  //private EventNotification? FromSyncState(SyncState state, INotificationEvent? @event) => @event switch
  //{
  //  PipelineStartReported e => new EventNotification("syncing.pipeline.started", new { Id = e.PipelineId.Name, StartedAt = @event.OccurredAt }),
  //  PipelineCompleteReported e => new EventNotification("syncing.pipeline.completed", new { Id = e.PipelineId.Name, FinishAt = @event.OccurredAt }),
  //  DashboardInitialized => new EventNotification("syncing.state.initialized", state),
  //  _ => null
  //};

  //private EventNotification? FromPipelineSchedules(PipelineSchedules schedules, INotificationEvent? @event) => @event switch
  //{
  //  SyncEngineReady e => new EventNotification("scheduler.specs.initialized", e.Schedules),
  //  PipelineScheduled e => new EventNotification("scheduler.jobs.added", schedules.Jobs.FirstOrDefault(j => j.Name == e.Name)!),
  //  PipelineTriggered e => new EventNotification("scheduler.jobs.removed", e.Name),
  //  DashboardInitialized => new EventNotification("scheduler.none", schedules),
  //  _ => null
  //};

  //private EventNotification? FromDiagnosisJournal(DiagnosisJournal journal, INotificationEvent? @event) => @event switch
  //{
  //  WorkerFailureReported
  //  or WorkerStartReported
  //  or WorkerCompleteReported
  //  or WorkerNonCreationReported
  //  or PipelineStartReported
  //  or PipelineSkipReported
  //  or PipelineCompleteReported => new EventNotification("diagnosis.records.added", journal.Records.LastOrDefault()!),
  //  DashboardInitialized => new EventNotification("diagnosis.records.initialized", journal),
  //  _ => null
  //};

  //private EventNotification? FromSwappingPlugins(RuntimePluginSet pluginSet, INotificationEvent? @event) => @event switch 
  //{
  //  PluginAdded => new EventNotification("pluginhub.entries.added", pluginSet.Entries.LastOrDefault()!),
  //  PluginRemoved e => new EventNotification("pluginhub.entries.removed", e.Name),
  //  PluginUpdated e => new EventNotification("pluginhub.entries.updated", new { e.Name, e.Version }),
  //  DashboardInitialized => new EventNotification("pluginhub.entries.initialized", pluginSet),
  //  PluginVersionsChecked e => new EventNotification("pluginhub.packages.checked", pluginSet.PackageEntries),
  //  _ => null
  //};
}
