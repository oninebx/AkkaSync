using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Schedules.Events;

// public sealed record SchedulerStarted(IReadOnlyDictionary<string, string> Specs) : ISyncEvent;

public sealed record PipelineTriggered(string Pid) : IProjectionEvent;
public sealed record SchedulerInitialized();
public sealed record DuplicateScheduleDetected(string Name) : IProjectionEvent;