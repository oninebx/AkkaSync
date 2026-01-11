using System;
using AkkaSync.Core.Application.Diagnosis;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Application.Store;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Infrastructure.Stores;

public class InMemoryDashboardStore : IDashboardStore
{
  private volatile HostSnapshot _snapshot = HostSnapshot.Empty;
  private volatile PipelineSchedules _schedules = PipelineSchedules.Empty;
  private volatile DiagnosisJournal _journal = DiagnosisJournal.Empty;

  public HostSnapshot Snapshot => _snapshot;

  public PipelineSchedules Schedules => _schedules;
  public DiagnosisJournal Journal => _journal;

  public IReadOnlyList<IStoreValue> GetEventsToReplay(long lastSeenSequence)
  {
    return [
      _snapshot,
      _schedules,
      _journal
    ];
  }

  public void Update<TValue>(TValue state) where TValue : IStoreValue
  {
    switch (state)
    {
      case HostSnapshot snapshot:
        _snapshot = snapshot;
        break;
      case PipelineSchedules schedules:
        _schedules = schedules;
        break;
      case DiagnosisJournal journal:
        _journal = journal;
        break;
      default:
        throw new InvalidOperationException($"Unsupported store value type: {typeof(TValue).Name}");
    }
  }
}

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
