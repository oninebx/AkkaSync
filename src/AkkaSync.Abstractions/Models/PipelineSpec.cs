using System;

namespace AkkaSync.Abstractions.Models;

public record PipelineSpec 
{
  public required string Name { get; init; }
  public required string Schedule { get; init; } = "0 0 * * *";
  public bool AutoStart { get; init; } = true;
  public required PluginSpec SourceProvider { get; init; }
  public required PluginSpec TransformerProvider { get; init; }
  public required PluginSpec SinkProvider { get; init; }
  public required PluginSpec HistoryStoreProvider { get; init; }
  public required List<string> DependsOn { get; init; } = [];
}

public record PluginSpec
{
  public required string Type { get; init; }
  public IReadOnlyDictionary<string, string> Parameters { get; init; } = new Dictionary<string, string>();
}