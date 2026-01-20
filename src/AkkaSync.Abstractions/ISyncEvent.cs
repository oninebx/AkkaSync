using System;

namespace AkkaSync.Abstractions;

public interface ISyncEvent
{
  DateTimeOffset Timestamp {get => DateTimeOffset.UtcNow; }
}
