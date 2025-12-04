using System;

namespace AkkaSync.Abstractions.Models;

public record PipelineContext{
  public required string Name { get; init; }
  public required PluginContext SourceProvider { get; init; }
  public required PluginContext TransformerProvider { get; init; }
  public required PluginContext SinkProvider { get; init; }
  public required PluginContext HistoryStoreProvider { get; init; }
  public required List<string> DependsOn { get; init; } = [];
}

public record PluginContext
{
  public required string Type { get; init; }
  public IReadOnlyDictionary<string, string> Parameters { get; init; } = new Dictionary<string, string>();
}