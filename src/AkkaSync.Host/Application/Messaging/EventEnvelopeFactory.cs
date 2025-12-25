using System;

namespace AkkaSync.Host.Application.Messaging;

public class EventEnvelopeFactory : IEventEnvelopeFactory
{
  private readonly ISequenceGenerator _sequenceGenerator;
  private readonly IEventIdGenerator _eventIdGenerator;
  public EventEnvelopeFactory(ISequenceGenerator sequenceGenerator, IEventIdGenerator eventIdGenerator)
  {
    _sequenceGenerator = sequenceGenerator;
    _eventIdGenerator = eventIdGenerator;
  }
  public EventEnvelope<T> Create<T>(string eventType, T payload, DateTimeOffset occurredAt) where T : IDashboardEvent
  {
    if(payload is null)
    {
      throw new ArgumentNullException(nameof(payload));
    }
    return new EventEnvelope<T>(
      _eventIdGenerator.NewId(), 
      eventType,
      _sequenceGenerator.Next(),
      payload,
      occurredAt);
  }
}
