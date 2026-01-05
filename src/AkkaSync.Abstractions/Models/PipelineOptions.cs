using System;

namespace AkkaSync.Abstractions.Models;

public record PipelineOptions
{
  public IReadOnlyList<PipelineSpec> Pipelines { get; init; } = [];

}
