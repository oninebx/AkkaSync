using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Workers.Events;

public sealed record WorkerStarted(WorkerId WorkerId) : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}
public sealed record WorkerCompleted(WorkerId WorkerId, string Etag) : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}
public sealed record WorkerFailed(WorkerId WorkerId, string Reason) : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}
public sealed record WorkerProgressed(WorkerId WorkerId, string Cursor) : ISyncEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}