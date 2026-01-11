using System;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Core.Domain.Pipelines;

public readonly record struct PipelineId(RunId RunId, string Name)
{
  public override string ToString() => $"{Name}-{RunId}";
}
