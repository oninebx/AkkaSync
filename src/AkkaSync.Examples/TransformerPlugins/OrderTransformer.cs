using System;
using AkkaSync.Core.Abstractions;
using AkkaSync.Core.Models;

namespace AkkaSync.Examples.TransformerPlugins;

public class OrderTransformer : TransformerBase
{
  protected override TransformContext Process(TransformContext item)
  {
    if(!item.TablesData.TryGetValue("_rawCsv", out var tableData))
    {
      throw new InvalidOperationException("Raw CSV data not found.");
    }
    var row = new Dictionary<string, object>
    {
      ["Id"] = tableData.GetValueOrDefault("Id", DBNull.Value),
      ["CustomerId"] = tableData.GetValueOrDefault("CustomerId", DBNull.Value),
      ["OrderDate"] = DateTime.TryParse(tableData.GetValueOrDefault("OrderDate", string.Empty)?.ToString(), out var orderDate) ? orderDate : DBNull.Value,
      ["TotalAmount"] = decimal.TryParse(tableData.GetValueOrDefault("TotalAmount", string.Empty)?.ToString(), out var totalAmount) ? totalAmount : DBNull.Value,
      ["Status"] = tableData.GetValueOrDefault("Status", DBNull.Value),
      ["ShippingAddress"] = tableData.GetValueOrDefault("ShippingAddress", DBNull.Value),
      ["Country"] = tableData.GetValueOrDefault("Country", DBNull.Value),
      ["IsPriority"] = bool.TryParse(tableData.GetValueOrDefault("IsPriority", "false")?.ToString(), out var isPriority) ? isPriority : false
    };

    item.TablesData["Order"] = row;
    item.TablesData.Remove("_rawCsv");

    return item;
  }
}
