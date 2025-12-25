using System;

namespace AkkaSync.Host.Application.Messaging;

public interface IEventEnvelopePublisher
{
  Task PublishAsync<T>(EventEnvelope<T> envelope) where T : IDashboardEvent;
}
