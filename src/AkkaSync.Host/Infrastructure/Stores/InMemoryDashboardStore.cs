using System;
using AkkaSync.Host.Application.Diagnosing;
using AkkaSync.Host.Application.Scheduling;
using AkkaSync.Host.Application.Syncing;
using AkkaSync.Host.Application.Swapping;
using AkkaSync.Infrastructure.Messaging.Publish;
using AkkaSync.Host.Application.Pipeline;
using AkkaSync.Host.Application.Plugin;
using AkkaSync.Host.Application.SyncWorker;

namespace AkkaSync.Host.Infrastructure.Stores;

public class InMemoryDashboardStore : IDashboardStore
{
  private volatile PipelineState _pipelineState = PipelineState.Empty;
  private volatile SyncState _syncState = SyncState.Empty;
  private volatile ScheduleState _schedules = ScheduleState.Empty;
  private volatile DiagnosisJournal _journal = DiagnosisJournal.Empty;
  private volatile PluginState _pluginSet = PluginState.EMPTY;
  private volatile WorkerState _workerState = WorkerState.Empty;
  private volatile SyncingState _syncingState = SyncingState.Empty;

  public IReadOnlyList<IStoreValue> GetEventsToReplay(long lastSeenSequence)
  {
    return [
      _pipelineState,
      _syncState,
      _schedules,
      _journal.ToShow(50),
      _pluginSet,
      _workerState,
      _syncingState,
    ];
  }

  public void Update<TValue>(TValue state) where TValue : IStoreValue
  {
    switch (state)
    {
      case PipelineState pipelineState:
        _pipelineState = pipelineState;
        break;
      case SyncState syncState:
        _syncState = syncState;
        break;
      case ScheduleState schedules:
        _schedules = schedules;
        break;
      case DiagnosisJournal journal:
        _journal = journal;
        break;
      case PluginState pluginSet:
        _pluginSet = pluginSet;
        break;
      case WorkerState workerState:
        _workerState = workerState;
        break;
      case SyncingState syncingState:
        _syncingState = syncingState;
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
