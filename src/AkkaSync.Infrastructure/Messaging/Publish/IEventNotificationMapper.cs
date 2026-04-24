using AkkaSync.Abstractions;

namespace AkkaSync.Infrastructure.Messaging.Publish
{
  public interface IEventNotificationMapper
  {
    EventNotification? TryMap(IStateSnashot value, IProjectionEvent? @event = null);
  }
}
