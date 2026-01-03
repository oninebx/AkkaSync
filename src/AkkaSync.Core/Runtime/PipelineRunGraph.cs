using System;
using Akka.Actor;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Pipeline;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Core.Runtime;

public sealed class PipelineRunGraph
{
  private IReadOnlyList<IReadOnlySet<string>> _layers { get; }

  private PipelineRunGraph(IReadOnlyList<IReadOnlySet<string>> layers)
  {
    _layers = layers;
  }

  public IReadOnlySet<string> Layer(int index)
  {
    return _layers[index];
  }

  public int LayerCount => _layers.Count;

  public static PipelineRunGraph Create(IReadOnlyList<PipelineSpec> pipelineSpecs)
  {
    var pipelineDict = pipelineSpecs.ToDictionary(p => p.Name, p => p);
    foreach (var pipeline in pipelineSpecs)
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

    while (processed.Count < pipelineSpecs.Count)
    {
      var currentLayer = new HashSet<string>();

      foreach (var pipeline in pipelineSpecs)
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

    return new PipelineRunGraph(layers);
  }
}
