using AkkaSync.Host.Application.Messaging;
using System;

namespace AkkaSync.Host.Infrastructure.Messaging;

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
    ArgumentNullException.ThrowIfNull(payload);
    return new EventEnvelope(
      _eventIdGenerator.NewId(), 
      eventType,
      _sequenceGenerator.Next(),
      payload,
      occurredAt);
  }
}
