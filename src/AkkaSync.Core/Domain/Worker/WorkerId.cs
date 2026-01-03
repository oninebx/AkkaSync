using System;
using AkkaSync.Core.Domain.Pipeline;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Core.Domain.Worker;

public readonly record struct WorkerId(
  PipelineId PipelineId,
  string SourceId)
{
  public override string ToString() => $"{PipelineId}-{SourceId}";
}
