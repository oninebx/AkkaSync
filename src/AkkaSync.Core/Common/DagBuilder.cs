using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Common;

public static class DagBuilder
{
  public static IReadOnlyList<IReadOnlyList<ISyncTransform>> Build(
      IEnumerable<ISyncTransform> transformers, string initialDependency)
  {
    var produced = new HashSet<string> { initialDependency };
    var remaining = transformers.ToList();
    var layers = new List<IReadOnlyList<ISyncTransform>>();

    while (remaining.Count != 0)
    {
      var layer = remaining
          .Where(t => t.DependsOn.All(d => produced.Contains(d)) || t.DependsOn.Length == 0)
          .ToList();

      if (layer.Count == 0)
        throw new InvalidOperationException("Circular dependency detected.");

      layers.Add(layer);

      foreach (var t in layer)
        produced.Add(t.Produce);

      remaining.RemoveAll(t => layer.Contains(t));
    }

    return layers;
  }

  //  public static IReadOnlyList<PluginNode> BuildGraphWithSourceSink(
  //    IReadOnlyList<ISyncSource> sources,
  //    IReadOnlyList<IReadOnlyList<ISyncTransformer>> tranformerLayers,
  //    ISyncSink sink,
  //    string pipelineId)
  //  {
  //    //var nodes = new Dictionary<string, PluginNode>();
  //    //foreach (var source in sources) 
  //    //{
  //    //  nodes[source.Id] = new PluginNode(source.QualifiedId, source.Name, "source", pipelineId);
  //    //}


  //    var instances = new List<PluginInstance>();
  //    var edges = new List<PluginEdge>();

  //    foreach (var source in sources)
  //    {
  //      instances.Add(new PluginInstance(source.Id, pipelineId, source.Key, "source"));
  //    }

  //    var allTransformers = tranformerLayers.SelectMany(l => l).ToList();
  //    foreach (var t in allTransformers)
  //    {
  //      instances.Add(new PluginInstance(t.Produce, pipelineId, t.Produce, "transformer"));
  //    }
  //    instances.Add(new PluginInstance(sink.Key, pipelineId, sink.Key, "sink"));

  //    foreach (var t in allTransformers)
  //    {
  //      foreach (var dep in t.DependsOn ?? [])
  //      {
  //        edges.Add(new PluginEdge($"{dep}->{t.Produce}", pipelineId, dep, t.Produce));
  //      }
  //    }

  //    var firstLayer = tranformerLayers.FirstOrDefault();
  //    if (firstLayer != null)
  //    {
  //      foreach (var source in sources)
  //      {
  //        foreach (var t in firstLayer)
  //        {
  //          if (t.DependsOn.Contains(source.Key))
  //          {
  //            edges.Add(new PluginEdge($"{source.Key}->{t.Produce}", pipelineId, source.Id, t.Produce));
  //          }
  //        }
  //      }
  //    }

  //    var lastLayer = tranformerLayers.LastOrDefault();
  //    if (lastLayer != null)
  //    {
  //      foreach (var t in lastLayer)
  //      {
  //        edges.Add(new PluginEdge($"{t.Produce}->{sink.Key}", pipelineId, t.Produce, sink.Key));
  //      }
  //    }

  //    return (instances, edges);
  //  }
}



// public sealed class TransformerDagBuilder
// {
//     public ISyncTransformer BuildChain(
//         IReadOnlyList<ISyncTransformer> transformers)
//     {
//         var produced = new HashSet<string>();
//         var remaining = transformers.ToList();

//         ISyncTransformer? head = null;
//         ISyncTransformer? prev = null;

//         while (remaining.Any())
//         {
//             var next = remaining.FirstOrDefault(t =>
//                 t.DependsOn.All(d => produced.Contains(d)));

//             if (next == null)
//                 throw new InvalidOperationException("Circular dependency detected");

//             if (head == null)
//                 head = next;
//             else
//                 prev!.SetNext(next);

//             produced.Add(next.Name);
//             prev = next;
//             remaining.Remove(next);
//         }

//         return head!;
//     }
// }
