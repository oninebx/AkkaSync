using System;

namespace AkkaSync.Abstractions;

public interface INotificationEvent
{
  DateTimeOffset OccurredAt { get => DateTimeOffset.UtcNow; }
}
