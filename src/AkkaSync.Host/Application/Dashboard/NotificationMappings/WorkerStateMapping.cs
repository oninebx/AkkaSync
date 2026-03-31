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
        WorkerStartReported e => new EventNotification("runningworker.worker.start", new { e.WorkerId, StartedAt = @event.OccurredAt }),
        WorkerErrorReported e => new EventNotification("runningworker.worker.error", state.Workers[e.WorkerId].ErrorCounts),
        _ => null
      };
    }
  }
}
