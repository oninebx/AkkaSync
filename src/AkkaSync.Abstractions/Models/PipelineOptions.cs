using System;

namespace AkkaSync.Abstractions.Models;

public record PipelineOptions
{
  public IReadOnlyDictionary<string, PipelineSpec>? Pipelines { get; init; }

}
