using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Core.Domain.Pipelines.Events;

public sealed record PipelineStarted(PipelineId PipelineId) : ISyncEvent;

public sealed record PipelineCompleted(PipelineId PipelineId) : ISyncEvent;