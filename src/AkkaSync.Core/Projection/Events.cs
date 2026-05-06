using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Workers;

namespace AkkaSync.Core.Notifications;

//public sealed record PipelineCreatedReported(PipelineId PipelineId, IReadOnlyList<PluginInstance> SourceInstances, IReadOnlyList<PluginInstance> TransformerInstances, PluginInstance SinkInstance) : IProjectionEvent;
//public sealed record WorkerMetricsReported(WorkerId WorkerId, IReadOnlyList<PluginMetrics> MetricsList) : IProjectionEvent;

//public sealed record PipelineStartReported(PipelineId PipelineId) : IProjectionEvent;
public sealed record PipelineSkipReported(PipelineId PipelineId, string Reason) : IProjectionEvent;
public sealed record PipelineCompleteReported(PipelineId PipelineId) : IProjectionEvent;
public sealed record WorkerFailureReported(WorkerId WorkerId, string Message) : IProjectionEvent;
public sealed record WorkerStartReported(WorkerId WorkerId) : IProjectionEvent;
public sealed record WorkerCompleteReported(WorkerId WorkerId) : IProjectionEvent;
public sealed record WorkerNonCreationReported(string SourceName) : IProjectionEvent;
public sealed record WorkerErrorReported(WorkerId WorkerId, IReadOnlyDictionary<string, int> ErrorStats) : IProjectionEvent;
