using System;

namespace AkkaSync.Host.Application.Messaging;

public interface IEventEnvelopeFactory
{
  EventEnvelope Create(string eventType, DashboardEvent payload, DateTimeOffset occurredAt);
}
