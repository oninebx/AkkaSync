using System;
using System.Text.Json.Serialization;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Workers;

namespace AkkaSync.Core.Application.Diagnosis;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DiagnosisLevel
{
    Info,
    Warning,
    Error
}

public sealed record DiagnosisRecord(DiagnosisLevel Level, string Message)
{
    public DateTimeOffset Timestamp { get; init; }
    public PipelineId? PipelineId { get; init; }
    public WorkerId? WorkerId { get; init; }
}
