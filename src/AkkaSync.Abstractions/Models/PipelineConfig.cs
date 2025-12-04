using System;

namespace AkkaSync.Abstractions.Models;

public record PipelineConfig
{
  public IReadOnlyList<PipelineContext> Pipelines { get; init; } = [];

  public IReadOnlyList<IReadOnlySet<string>> BuildLayers()
  {
    var pipelineDict = Pipelines.ToDictionary(p => p.Name, p => p);
    foreach (var pipeline in Pipelines)
    {
      foreach (var dep in pipeline.DependsOn)
      {
        if (!pipelineDict.ContainsKey(dep))
        {
          throw new InvalidOperationException($"Dependency '{dep}' of pipeline '{pipeline.Name}' not found in configuration.");
        }
      }
    }

    var layers = new List<HashSet<string>>();
    var processed = new HashSet<string>();

    while (processed.Count < Pipelines.Count)
    {
      var currentLayer = new HashSet<string>();

      foreach (var pipeline in Pipelines)
      {
        if (processed.Contains(pipeline.Name))
          continue;

        if (pipeline.DependsOn.All(d => processed.Contains(d)))
        {
          currentLayer.Add(pipeline.Name);
        }
      }

      if (currentLayer.Count == 0)
        throw new InvalidOperationException("Cyclic dependency detected among pipelines.");

      layers.Add(currentLayer);
      foreach (var name in currentLayer)
      {
        processed.Add(name);
      }
    }

    return layers;
  }

}
