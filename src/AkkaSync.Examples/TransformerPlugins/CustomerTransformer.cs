using System;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Examples.TransformerPlugins;

public class CustomerTransformer : TransformerBase
{
  protected override TransformContext Process(TransformContext context)
  {
    if(!context.TablesData.TryGetValue("_rawCsv", out var tableData))
    {
      throw new InvalidOperationException("Raw CSV data not found.");
    }
    var row = new Dictionary<string, object>
    {
      ["Id"] = tableData.GetValueOrDefault("Id", DBNull.Value),
      ["FirstName"] = tableData.GetValueOrDefault("FirstName", DBNull.Value),
      ["LastName"] = tableData.GetValueOrDefault("LastName", DBNull.Value),
      ["Email"] = tableData.GetValueOrDefault("Email", DBNull.Value),
      ["Phone"] = tableData.GetValueOrDefault("Phone", DBNull.Value),
      ["DateOfBirth"] = DateTime.TryParse(tableData.GetValueOrDefault("DateOfBirth", string.Empty)?.ToString(), out var dob) ? dob : (object)DBNull.Value,
      ["Country"] = tableData.GetValueOrDefault("Country", DBNull.Value),
      ["IsActive"] = bool.TryParse(tableData.GetValueOrDefault("IsActive", "false")?.ToString(), out var isActive) && isActive
    };

    context.TablesData["Customer"] = row;
    context.TablesData.Remove("_rawCsv");

    return context;
  }
}
