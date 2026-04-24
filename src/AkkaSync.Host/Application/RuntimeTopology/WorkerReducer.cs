using AkkaSync.Abstractions;
using AkkaSync.Core.Notifications;
using AkkaSync.Host.Application.SyncWorker;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.RunningWorker
{
  public static class WorkerReducer
  {
    public static WorkerState Reduce(WorkerState current, IProjectionEvent @event) => @event switch
    {
      WorkerStartReported e => current with
      {
        Workers = current.Workers.Add(e.WorkerId, new WorkerRecord(e.WorkerId) { StartedAt =  @event.OccurredAt}),
      },
      WorkerErrorReported e => current with
      {
        Workers = current.Workers.SetItem(e.WorkerId, current.Workers[e.WorkerId] with { ErrorCounts = MergeErrorCounts(current.Workers[e.WorkerId].ErrorCounts, e.ErrorStats) }),
      },
      _ => current
    };

    //private static ImmutableDictionary<string, long> UpdateErrorCounts(ImmutableDictionary<string, long> current, IReadOnlyDictionary<string, int> newErrors)
    //{
    //  var builder = current.ToBuilder();
    //  foreach(var kvp in newErrors)
    //  {
    //    if(builder.ContainsKey(kvp.Key))
    //    {
    //      builder[kvp.Key] += kvp.Value;
    //    }
    //    else
    //    {
    //      builder[kvp.Key] = kvp.Value;
    //    }
    //  }
    //  return builder.ToImmutable();
    //}

    private static ImmutableDictionary<string, long> MergeErrorCounts(
    ImmutableDictionary<string, long> current,
    IReadOnlyDictionary<string, int> incoming)
    {
      return incoming.Aggregate(current, (acc, kv) =>
          acc.SetItem(kv.Key, acc.GetValueOrDefault(kv.Key) + kv.Value)
      );
    }
  }
}
