using System;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Notifications;

namespace AkkaSync.Core.Domain.Shared;

public sealed record SyncEngineReady(IReadOnlyList<PipelineSpec> Pipelines, IReadOnlyDictionary<string, ScheduleSpec> Schedules) : IProjectionEvent;
public sealed record SyncEngineStopped();
