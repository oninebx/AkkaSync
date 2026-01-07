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
  public EventEnvelope Create(string eventType, DashboardEvent payload, DateTimeOffset occurredAt)
  {
    if(payload is null)
    {
      throw new ArgumentNullException(nameof(payload));
    }
    return new EventEnvelope(
      _eventIdGenerator.NewId(), 
      eventType,
      _sequenceGenerator.Next(),
      payload,
      occurredAt);
  }
}
