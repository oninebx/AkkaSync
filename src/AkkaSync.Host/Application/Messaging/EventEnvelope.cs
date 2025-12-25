using System;

namespace AkkaSync.Host.Application.Messaging;

public record EventEnvelope<TEvent>(
  string Id,
  string Type,
  long Sequence,
  TEvent Event,
  DateTimeOffset OccurredAt
) where TEvent: IDashboardEvent;
