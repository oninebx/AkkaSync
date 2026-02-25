using System;
using AkkaSync.Host.Application.Diagnosing;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Application.Scheduling;
using AkkaSync.Host.Application.Syncing;
using AkkaSync.Host.Application.Swapping;

namespace AkkaSync.Host.Infrastructure.Stores;

public class InMemoryDashboardStore : IDashboardStore
{
  private volatile SyncState _syncState = SyncState.Empty;
  private volatile PipelineSchedules _schedules = PipelineSchedules.Empty;
  private volatile DiagnosisJournal _journal = DiagnosisJournal.Empty;
  private volatile RuntimePluginSet _pluginSet = RuntimePluginSet.EMPTY;

  public SyncState SyncState => _syncState;
  public PipelineSchedules Schedules => _schedules;
  public DiagnosisJournal Journal => _journal;
  public RuntimePluginSet PluginSet => _pluginSet;

  public IReadOnlyList<IStoreValue> GetEventsToReplay(long lastSeenSequence)
  {
    return [
      _syncState,
      _schedules,
      _journal.ToShow(50),
      _pluginSet,
    ];
  }

  public void Update<TValue>(TValue state) where TValue : IStoreValue
  {
    switch (state)
    {
      case SyncState syncState:
        _syncState = syncState;
        break;
      case PipelineSchedules schedules:
        _schedules = schedules;
        break;
      case DiagnosisJournal journal:
        _journal = journal;
        break;
      case RuntimePluginSet pluginSet:
        _pluginSet = pluginSet;
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
