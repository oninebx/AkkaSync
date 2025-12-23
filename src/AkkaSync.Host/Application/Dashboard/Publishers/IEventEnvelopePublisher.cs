using System;
using AkkaSync.Host.Application.Dashboard.Events;

namespace AkkaSync.Host.Application.Dashboard.Publishers;

public interface IEventEnvelopePublisher : IReadModelNotifier<EventEnvelope>
{
  Task RegisterClientAsync(string connectionId, long lastSeenSequence);
  void UnregisterClient(string connectionId);
  Task PublishAsync(EventEnvelope message) => NotifyAsync(message);
}
