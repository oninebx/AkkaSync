using AkkaSync.Abstractions;
using AkkaSync.Core.Notifications;
using AkkaSync.Host.Application.SyncWorker;
using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Host.Application.Dashboard.NotificationMappings
{
  public sealed class WorkerStateMapping : IEventNotificationMapping
  {
    public bool CanHandle(IStoreValue value) => value is WorkerState;

    public EventNotification? TryMap(IStoreValue store, INotificationEvent? @event)
    {
      var state = (WorkerState)store;
      return @event switch
      {
        DashboardInitialized e => new EventNotification("worker.state.initialized",state.Workers.Select(kvp => new { PipelineId = kvp.Key.PipelineId.Key, Id = kvp.Key.SourceId, kvp.Value.StartedAt, kvp.Value.ErrorCounts })),
        WorkerStartReported e => new EventNotification("worker.records.added", new { PipelineId = e.WorkerId.PipelineId.Key, Id = e.WorkerId.SourceId, StartedAt = @event.OccurredAt }),
        WorkerErrorReported e => new EventNotification("worker.errors.added", new { PipelineId = e.WorkerId.PipelineId.Key, Id = e.WorkerId.SourceId, state.Workers[e.WorkerId].ErrorCounts }),
        _ => null
      };
    }
  }
}
