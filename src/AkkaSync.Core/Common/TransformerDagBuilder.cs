using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Common;

public static class TransformerDagBuilder
{
  public static IReadOnlyList<IReadOnlyList<ISyncTransformer>> Build(
      IEnumerable<ISyncTransformer> transformers)
  {
    var produced = new HashSet<string>();
    var remaining = transformers.ToList();
    var layers = new List<IReadOnlyList<ISyncTransformer>>();

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
