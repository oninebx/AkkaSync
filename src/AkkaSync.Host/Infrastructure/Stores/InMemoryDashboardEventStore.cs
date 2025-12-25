using System;
using System.Collections.Immutable;
using AkkaSync.Host.Application.Dashboard.Stores;

namespace AkkaSync.Host.Infrastructure.Stores;

// public class InMemoryDashboardEventStore : IDashboardEventStore
// {
//   private const int CAPACITY = 100;
//   private long _sequence = 0;
//   private ImmutableList<EventEnvelope> _events = [];

//   public event Func<EventEnvelope, Task>? OnAppended;

//   public void Append(IDashboardEvent @event)
//   {
//     var envelope =  @event switch
//     {
//       DashboardPipelineStarted e => new EventEnvelope(Interlocked.Increment(ref _sequence)),
//       _ => throw new NotSupportedException(@event.GetType().Name)
//     };

//     ImmutableInterlocked.Update(ref _events, list =>
//     {
//       var next = list.Add(envelope);
//       return next.Count > CAPACITY ? next.RemoveRange(0, next.Count - CAPACITY) : next;
//     });

//     _ = OnAppended?.Invoke(envelope);
//   }

//   public IReadOnlyList<EventEnvelope> GetAfter(long sequence)
//     => [.. _events.Where(e => e.Sequence > sequence)];

//   public IReadOnlyList<EventEnvelope> GetLatest(int max = 50)
//     => [.. _events.TakeLast(max)];

//   public long? GetMinSequence() => _events.FirstOrDefault()?.Sequence;
// }
