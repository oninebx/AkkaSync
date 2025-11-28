using System;
using AkkaSync.Core.Pipeline;

namespace AkkaSync.Examples.TransformerPlugins;

public class ProductTransformer : TransformerBase
{
  protected override TransformContext Process(TransformContext item)
  {
    if (!item.TablesData.TryGetValue("_rawCsv", out var tableData))
        throw new InvalidOperationException("Raw CSV data not found.");

    var row = new Dictionary<string, object>
    {
        ["Id"] = tableData.GetValueOrDefault("Id", DBNull.Value),
        ["Name"] = tableData.GetValueOrDefault("Name", DBNull.Value),
        ["Price"] = decimal.TryParse(tableData.GetValueOrDefault("Price", "0")?.ToString(), out var price) ? price : 0m
    };

    item.TablesData["Product"] = row;
    item.TablesData.Remove("_rawCsv");

    return item;
  }
}
