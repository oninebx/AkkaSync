using System;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Plugins.Transformer.examples;

public class CsvTransformerProvider : IPluginProvider<ISyncTransformer>
{
  public string Key => nameof(CsvTransformerProvider);

  public IEnumerable<ISyncTransformer> Create(PluginSpec context,CancellationToken cancellationToken)
  {
    switch (context.Parameters["transform"])
    {
      case "customer":
        yield return new CustomerTransformer();
        yield break;
      case "order":
        yield return new OrderTransformer();
        yield break;
      case "product":
        yield return new ProductTransformer();
        yield break;
      case "orderItem":
        yield return new OrderItemTransformer();
        yield break;
      case "audit":
        yield return new AuditTransformer();
        yield break;
      default:
        throw new NotSupportedException($"Transform type '{context.Parameters["transform"]}' is not supported.");
    }
  }
}
