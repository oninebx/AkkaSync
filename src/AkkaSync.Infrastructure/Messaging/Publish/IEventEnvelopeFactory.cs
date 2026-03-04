using System;

namespace AkkaSync.Infrastructure.Messaging.Publish;

public interface IEventEnvelopeFactory
{
  EventEnvelope Create(string eventType, EventNotification payload, DateTimeOffset occurredAt);
}
