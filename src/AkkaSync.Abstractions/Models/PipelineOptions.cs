using System;

namespace AkkaSync.Abstractions.Models;

public record PipelineOptions
{
  public IDictionary<string, PipelineSpec>? Pipelines { get; init; }

}
