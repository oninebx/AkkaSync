using System;

namespace AkkaSync.Infrastructure.Messaging.Publish;

public record EventEnvelope(
  string Id,
  string Type,
  long Sequence,
  EventNotification Event,
  DateTimeOffset OccurredAt
);
