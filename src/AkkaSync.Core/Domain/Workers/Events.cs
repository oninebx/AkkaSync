using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Workers.Events;

public sealed record WorkerStarted(WorkerId WorkerId);
public sealed record WorkerCompleted(WorkerId WorkerId, string Etag);
public sealed record WorkerFailed(WorkerId WorkerId, string Reason);
public sealed record WorkerProgressed(WorkerId WorkerId, string Cursor);