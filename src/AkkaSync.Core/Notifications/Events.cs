using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Workers;

namespace AkkaSync.Core.Notifications;

public sealed record PipelineStartReported(PipelineId PipelineId) : INotificationEvent;
public sealed record PipelineCompleteReported(PipelineId PipelineId) : INotificationEvent;
public sealed record WorkerFailureReported(WorkerId WorkerId, string Message) : INotificationEvent;
public sealed record WorkerStartReported(WorkerId WorkerId) : INotificationEvent;
public sealed record WorkerCompleteReported(WorkerId WorkerId) : INotificationEvent;
