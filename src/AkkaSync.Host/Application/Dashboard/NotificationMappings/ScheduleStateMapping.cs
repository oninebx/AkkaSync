using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Schedules.Events;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Host.Application.Scheduling;
using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Host.Application.Dashboard.NotificationMappings
{
  public sealed class ScheduleStateMapping : IEventNotificationMapping
  {
    public bool CanHandle(IStoreValue value) => value is ScheduleState;

    public EventNotification? TryMap(IStoreValue store, INotificationEvent? @event)
    {
      var state = (ScheduleState)store;
      return @event switch
      {
        SyncEngineReady e => new EventNotification("scheduler.specs.initialized", e.Schedules),
        PipelineScheduled e => new EventNotification("scheduler.jobs.added", state.Jobs.FirstOrDefault(j => j.Id == e.Name)!),
        PipelineTriggered e => new EventNotification("scheduler.jobs.removed", e.Pid),
        DashboardInitialized => new EventNotification("scheduler.all.notified", state),
        _ => null
      };
    }
  }
}
