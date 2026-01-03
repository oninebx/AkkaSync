using System;

namespace AkkaSync.Abstractions.Models;

public record AkkaSyncOptions
{
  public IReadOnlyList<PipelineSpec> Pipelines { get; init; } = [];

}
