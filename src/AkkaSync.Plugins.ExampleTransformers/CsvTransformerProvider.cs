using System;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Plugins.ExampleTransformers;

namespace AkkaSync.Plugins.Transformer.examples;

public class CsvTransformerProvider : IPluginProvider<ISyncTransformer>
{
  public string Key => nameof(CsvTransformerProvider);

  public IEnumerable<ISyncTransformer> Create(PluginSpec context,CancellationToken cancellationToken)
  {
    switch (context.Parameters["transform"])
    {
      case "customer":
        yield return new TableTransformer(
            "_rawCsv",
            "Customer",
            row => new Dictionary<string, object?>
            {
              ["Id"] = row.GetValueOrDefault("Id", DBNull.Value),
              ["FirstName"] = row.GetValueOrDefault("FirstName", DBNull.Value),
              ["LastName"] = row.GetValueOrDefault("LastName", DBNull.Value),
              ["Email"] = row.GetValueOrDefault("Email", DBNull.Value),
              ["Phone"] = row.GetValueOrDefault("Phone", DBNull.Value),
              ["DateOfBirth"] = DateTime.TryParse(row.GetValueOrDefault("DateOfBirth", string.Empty)?.ToString(), out var dob) ? dob : (object)DBNull.Value,
              ["Country"] = row.GetValueOrDefault("Country", DBNull.Value),
              ["IsActive"] = bool.TryParse(row.GetValueOrDefault("IsActive", "false")?.ToString(), out var isActive) && isActive
            });
        yield break;
      case "product":
        yield return new TableTransformer(
            "_rawCsv",
            "Product",
            row => new Dictionary<string, object?>
            {
              ["Id"] = row.GetValueOrDefault("Id", DBNull.Value),
              ["Name"] = row.GetValueOrDefault("Name", DBNull.Value),
              ["Description"] = row.GetValueOrDefault("Description", DBNull.Value),
              ["Price"] = decimal.TryParse(row.GetValueOrDefault("Price", "0")?.ToString(), out var price) ? price : 0m,
              ["Stock"] = int.TryParse(row.GetValueOrDefault("Stock", "0")?.ToString(), out var stock) ? stock : 0
            });
        yield break;
      case "order":
        yield return new TableTransformer(
            "_rawCsv",
            "Order",
            row => new Dictionary<string, object?>
            {
              ["Id"] = row.GetValueOrDefault("Id", DBNull.Value),
              ["CustomerId"] = row.GetValueOrDefault("CustomerId", DBNull.Value),
              ["OrderDate"] = DateTime.TryParse(row.GetValueOrDefault("OrderDate", string.Empty)?.ToString(), out var orderDate) ? orderDate : (object)DBNull.Value,
              ["TotalAmount"] = decimal.TryParse(row.GetValueOrDefault("TotalAmount", "0")?.ToString(), out var totalAmount) ? totalAmount : 0m
            });
        yield break;
      case "orderItem":
        yield return new TableTransformer(
            "_rawCsv",
            "OrderItem",
            row => new Dictionary<string, object?>
            {
              ["Id"] = row.GetValueOrDefault("Id", DBNull.Value),
              ["OrderId"] = row.GetValueOrDefault("OrderId", DBNull.Value),
              ["ProductId"] = row.GetValueOrDefault("ProductId", DBNull.Value),
              ["Quantity"] = int.TryParse(row.GetValueOrDefault("Quantity", "0")?.ToString(), out var quantity) ? quantity : 0,
              ["UnitPrice"] = decimal.TryParse(row.GetValueOrDefault("UnitPrice", "0")?.ToString(), out var unitPrice) ? unitPrice : 0m
            });
        yield break;
      case "audit":
        yield return new TableTransformer(
            "_rawCsv",
            "Audit",
            row => new Dictionary<string, object?>
            {
              ["Id"] = row.GetValueOrDefault("AuditId", DBNull.Value),
              ["OrderId"] = row.GetValueOrDefault("OrderId", DBNull.Value),
              ["CustomerId"] = row.GetValueOrDefault("CustomerId", DBNull.Value),
              ["Action"] = row.GetValueOrDefault("Action", DBNull.Value),
              ["ActionDate"] = DateTime.TryParse(row.GetValueOrDefault("ActionDate", string.Empty)?.ToString(), out var dt) ? dt : DBNull.Value,
              ["Notes"] = row.GetValueOrDefault("Notes", DBNull.Value)
            });
        yield break;
      default:
        throw new NotSupportedException($"Transform type '{context.Parameters["transform"]}' is not supported.");
    }
  }
}
