using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Workers.Events;

public sealed record WorkerStarted(WorkerId WorkerId) : ISyncEvent;
public sealed record WorkerCompleted(WorkerId WorkerId, string Etag) : ISyncEvent;
public sealed record WorkerFailed(WorkerId WorkerId, string Reason) : ISyncEvent;
public sealed record WorkerProgressed(WorkerId WorkerId, string Cursor) : ISyncEvent;