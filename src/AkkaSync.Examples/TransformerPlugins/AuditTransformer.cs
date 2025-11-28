using System;
using AkkaSync.Core.Pipeline;

namespace AkkaSync.Examples.TransformerPlugins;

public class AuditTransformer : TransformerBase
{
  protected override TransformContext Process(TransformContext item)
  {
    if (!item.TablesData.TryGetValue("_rawCsv", out var tableData))
        throw new InvalidOperationException("Raw CSV data not found.");

    var row = new Dictionary<string, object>
    {
        ["Id"] = tableData.GetValueOrDefault("AuditId", DBNull.Value),
        ["OrderId"] = tableData.GetValueOrDefault("OrderId", DBNull.Value),
        ["CustomerId"] = tableData.GetValueOrDefault("CustomerId", DBNull.Value),
        ["Action"] = tableData.GetValueOrDefault("Action", DBNull.Value),
        ["ActionDate"] = DateTime.TryParse(tableData.GetValueOrDefault("ActionDate", string.Empty)?.ToString(), out var dt) ? dt : DBNull.Value,
        ["Notes"] = tableData.GetValueOrDefault("Notes", DBNull.Value)
    };

    item.TablesData["Audit"] = row;
    item.TablesData.Remove("_rawCsv");

    return item;
  }
}
