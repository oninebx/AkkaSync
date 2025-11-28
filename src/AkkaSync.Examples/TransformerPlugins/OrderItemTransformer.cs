using System;
using AkkaSync.Core.Pipeline;

namespace AkkaSync.Examples.TransformerPlugins;

public class OrderItemTransformer : TransformerBase
{
  protected override TransformContext Process(TransformContext item)
  {
    if (!item.TablesData.TryGetValue("_rawCsv", out var tableData))
        throw new InvalidOperationException("Raw CSV data not found.");

    var row = new Dictionary<string, object>
    {
        ["Id"] = tableData.GetValueOrDefault("Id", DBNull.Value),
        ["OrderId"] = tableData.GetValueOrDefault("OrderId", DBNull.Value),
        ["ProductId"] = tableData.GetValueOrDefault("ProductId", DBNull.Value),
        ["Quantity"] = int.TryParse(tableData.GetValueOrDefault("Quantity", "0")?.ToString(), out var qty) ? qty : 0,
        ["TotalPrice"] = decimal.TryParse(tableData.GetValueOrDefault("TotalPrice", "0")?.ToString(), out var total) ? total : 0m
    };

    item.TablesData["OrderItem"] = row;
    item.TablesData.Remove("_rawCsv");

    return item;
  }
}
