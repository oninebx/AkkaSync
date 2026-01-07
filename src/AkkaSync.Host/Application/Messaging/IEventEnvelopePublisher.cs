using System;

namespace AkkaSync.Host.Application.Messaging;

public interface IEventEnvelopePublisher
{
  Task PublishAsync(EventEnvelope envelope);
}
