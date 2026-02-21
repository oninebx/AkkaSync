using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Schedules.Events;

// public sealed record SchedulerStarted(IReadOnlyDictionary<string, string> Specs) : ISyncEvent;

public sealed record PipelineScheduled(string Name, DateTime NextUtc) : INotificationEvent;

public sealed record PipelineTriggered(string Name) : INotificationEvent;
public sealed record SchedulerInitialized();
public sealed record DuplicateScheduleDetected(string Name) : INotificationEvent;