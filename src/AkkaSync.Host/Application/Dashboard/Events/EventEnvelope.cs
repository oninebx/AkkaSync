using System;

namespace AkkaSync.Host.Application.Dashboard.Events;

public record EventEnvelope(long Sequence);
