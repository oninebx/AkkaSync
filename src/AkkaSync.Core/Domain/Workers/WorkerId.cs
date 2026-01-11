using System;
using AkkaSync.Core.Domain.Pipelines;

namespace AkkaSync.Core.Domain.Workers;

public readonly record struct WorkerId(
  PipelineId PipelineId,
  string SourceId)
{
  public override string ToString() => $"{PipelineId}-{SourceId}";
}
