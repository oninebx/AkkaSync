using System;
using AkkaSync.Core.Common;
using AkkaSync.Core.Configuration;
using AkkaSync.Core.Abstractions;
using AkkaSync.Core.Models;
using AkkaSync.Core.PluginProviders;

namespace AkkaSync.Examples.TransformerPlugins;

public class CsvTransformerProvider : IPluginProvider<ISyncTransformer>
{
  public string Key => nameof(CsvTransformerProvider);

  public IEnumerable<ISyncTransformer> Create(PipelineContext context,CancellationToken cancellationToken)
  {
    switch (context.TransformerProvider.Parameters["transform"])
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
        throw new NotSupportedException($"Transform type '{context.TransformerProvider.Parameters["transform"]}' is not supported.");
    }
  }
}
