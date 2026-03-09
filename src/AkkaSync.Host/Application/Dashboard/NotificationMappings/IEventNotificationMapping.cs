using AkkaSync.Abstractions;
using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Host.Application.Dashboard.NotificationMappings
{
  public interface IEventNotificationMapping
  {
    bool CanHandle(IStoreValue value);
    EventNotification? TryMap(IStoreValue store, INotificationEvent? @event);
  }
}
