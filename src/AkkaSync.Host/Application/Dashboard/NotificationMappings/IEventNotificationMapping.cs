using AkkaSync.Abstractions;
using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Host.Application.Dashboard.NotificationMappings
{
  public interface IEventNotificationMapping
  {
    bool CanHandle(IStateSnashot value);
    EventNotification? TryMap(IStateSnashot store, IProjectionEvent? @event);
  }
}
