using System;

namespace AkkaSync.Host.Application.Messaging;

public record EventEnvelope(
  string Id,
  string Type,
  long Sequence,
  DashboardEvent Event,
  DateTimeOffset OccurredAt
);
