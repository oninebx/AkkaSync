using AkkaSync.Core.Domain.Workers;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.SyncWorker
{
  public sealed record WorkerRecord(WorkerId Id)
  {
    public long ProcessedCount { get; init; } = 0;
    public ImmutableDictionary<string, long> ErrorCounts { get; init; } = ImmutableDictionary<string, long>.Empty;
    public double Throughput { get; init; } = 0d;
    public DateTimeOffset StartedAt { get; init; }
    public DateTimeOffset FinishedAt { get; init; }
  }
}
