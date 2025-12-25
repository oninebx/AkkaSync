using System;

namespace AkkaSync.Host.Application.Messaging;

public interface IEventEnvelopeFactory
{
  EventEnvelope<T> Create<T>(string eventType, T payload, DateTimeOffset occurredAt) where T: IDashboardEvent;
}
