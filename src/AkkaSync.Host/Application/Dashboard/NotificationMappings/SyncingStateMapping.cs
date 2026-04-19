using AkkaSync.Abstractions;
using AkkaSync.Core.Notifications;
using AkkaSync.Host.Application.Syncing;
using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Host.Application.Dashboard.NotificationMappings
{
  public class SyncingStateMapping : IEventNotificationMapping
  {
    public bool CanHandle(IStoreValue value) => value is SyncingState;

    public EventNotification? TryMap(IStoreValue store, INotificationEvent? @event)
    {
      var state = (SyncingState)store;
      return @event switch
      {
        PipelineCreatedReported e => new EventNotification("sync.plugins.added", state.Instances[e.PipelineId.Key]),
        _ => null
      };
    }
  }
}
