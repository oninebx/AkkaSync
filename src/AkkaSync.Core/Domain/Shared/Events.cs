using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Notifications;

namespace AkkaSync.Core.Domain.Shared;

public sealed record SyncEngineReady(IReadOnlyList<PipelineInfo> Pipelines, IReadOnlyDictionary<string, string> Schedules) : INotificationEvent;
public sealed record SyncEngineStopped();
