using AkkaSync.Abstractions;
using AkkaSync.Core.Notifications;
using AkkaSync.Core.Projection;
using AkkaSync.Host.Application.Syncing;
using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Host.Application.Dashboard.NotificationMappings
{
  public class SyncingStateMapping : IEventNotificationMapping
  {
    public bool CanHandle(IStateSnashot value) => value is SyncingState;

    public EventNotification? TryMap(IStateSnashot store, IProjectionEvent? @event)
    {
      var state = (SyncingState)store;
      return @event switch
      {
        PipelineCreatedTransition e => new EventNotification("sync.plugins.added", state.Instances[e.PipelineId.Key]),
        _ => null
      };
    }
  }
}
