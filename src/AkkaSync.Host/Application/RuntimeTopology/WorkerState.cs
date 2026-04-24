using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Workers;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.SyncWorker
{
  public sealed record WorkerState(ImmutableDictionary<WorkerId, WorkerRecord> Workers): IStateSnashot
  {
    public static WorkerState Empty => new(ImmutableDictionary<WorkerId, WorkerRecord>.Empty);
  }
}
