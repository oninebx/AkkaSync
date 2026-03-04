using System;

namespace AkkaSync.Infrastructure.Messaging.Publish;

public interface IEventEnvelopePublisher
{
  Task PublishAsync(EventEnvelope envelope);
}
