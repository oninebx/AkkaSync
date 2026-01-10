using System;

namespace AkkaSync.Host.Domain.Dashboard.ValueObjects;

public sealed record ErrorRecord(DateTimeOffset OccurredAt, string? Message = null);
