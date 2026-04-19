using AkkaSync.Core.Domain.Workers;
using AkkaSync.Infrastructure.Messaging.Publish;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.SyncWorker
{
  public sealed record WorkerState(ImmutableDictionary<WorkerId, WorkerRecord> Workers): IStoreValue
  {
    public static WorkerState Empty => new(ImmutableDictionary<WorkerId, WorkerRecord>.Empty);
  }
}
