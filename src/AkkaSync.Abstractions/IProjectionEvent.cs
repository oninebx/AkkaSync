using System;

namespace AkkaSync.Abstractions;

public interface IProjectionEvent
{
  DateTimeOffset OccurredAt { get => DateTimeOffset.UtcNow; }
}
