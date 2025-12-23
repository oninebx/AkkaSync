using System;
using System.Collections.Immutable;
using AkkaSync.Host.Application.Dashboard.Events;

namespace AkkaSync.Host.Application.Dashboard.Stores;

public interface IDashboardEventStore
{
  IReadOnlyList<EventEnvelope> GetLatest(int max = 50);
  IReadOnlyList<EventEnvelope> GetAfter(long sequence);
  void Append(IDashboardEvent @event);
  event Func<EventEnvelope, Task>? OnAppended;
  long? GetMinSequence();
}
