using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Workers;

namespace AkkaSync.Core.Notifications;

public sealed record PipelineCreatedReported(PipelineId PipelineId, IReadOnlyList<PluginInstance> SourceInstances, IReadOnlyList<PluginInstance> TransformerInstances, PluginInstance SinkInstance) : INotificationEvent;
public sealed record PipelineStartReported(PipelineId PipelineId) : INotificationEvent;
public sealed record PipelineSkipReported(PipelineId PipelineId, string Reason) : INotificationEvent;
public sealed record PipelineCompleteReported(PipelineId PipelineId) : INotificationEvent;
public sealed record WorkerFailureReported(WorkerId WorkerId, string Message) : INotificationEvent;
public sealed record WorkerStartReported(WorkerId WorkerId) : INotificationEvent;
public sealed record WorkerCompleteReported(WorkerId WorkerId) : INotificationEvent;
public sealed record WorkerNonCreationReported(string SourceName) : INotificationEvent;
public sealed record WorkerErrorReported(WorkerId WorkerId, IReadOnlyDictionary<string, int> ErrorStats) : INotificationEvent;
