using System;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Plugins.Transformer.DbTable;

namespace AkkaSync.Examples.CsvSqlite;

public class CsvTransformerProvider : IPluginProvider<ISyncTransformer>
{
  public string Key => nameof(CsvTransformerProvider);

  public IEnumerable<ISyncTransformer> Create(PluginSpec context, CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
    // switch (context.Parameters["transform"])
    // {
    //   case "customer":
    //     yield return new TableTransformer(
    //         "Customer",
    //         "customer.ready",
    //         new HashSet<string>(),
    //         row => new Dictionary<string, object?>
    //         {
    //           ["Id"] = row.GetValueOrDefault("customer_external_id", DBNull.Value),
    //           ["first_name"] = row.GetValueOrDefault("customer_first_name", DBNull.Value),
    //           ["last_name"] = row.GetValueOrDefault("customer_last_name", DBNull.Value),
    //           ["email"] = row.GetValueOrDefault("customer_email", DBNull.Value),
    //           ["phone"] = row.GetValueOrDefault("customer_phone", DBNull.Value),
    //           ["dob"] = row.GetValueOrDefault<DateTime>("customer_dob", DBNull.Value),
    //           // ["dob"] = DateTime.TryParse(row.GetValueOrDefault("customer_dob", string.Empty)?.ToString(), out var dob) ? dob : (object)DBNull.Value,
    //           ["country"] = row.GetValueOrDefault("customer_country", DBNull.Value),
    //           ["is_active"] = row.GetValueOrDefault<bool>("customer_is_active", false)
    //           // ["is_active"] = bool.TryParse(row.GetValueOrDefault("customer_is_active", "false")?.ToString(), out var isActive) && isActive
    //         });
    //     yield break;
    //   case "product":
    //     yield return new TableTransformer(
    //         "Product",
    //         "product.ready",
    //         new HashSet<string>(),
    //         row => new Dictionary<string, object?>
    //         {
    //           ["sku"] = row.GetValueOrDefault("product_sku", DBNull.Value),
    //           ["name"] = row.GetValueOrDefault("product_name", DBNull.Value),
    //           ["price"] = row.GetValueOrDefault<decimal>("product_price", 0m)
    //           // ["price"] = decimal.TryParse(row.GetValueOrDefault("product_price", "0")?.ToString(), out var price) ? price : 0m,
    //         });
    //     yield break;
    //   case "order":
    //     yield return new TableTransformer(
    //         "Order",
    //         "order.ready",
    //         new HashSet<string> { "customer.ready" },
    //         row => new Dictionary<string, object?>
    //         {
    //           ["id"] = row.GetValueOrDefault("order_external_id", DBNull.Value),
    //           ["customer_id"] = row.GetValueOrDefault("CustomerId", DBNull.Value),
    //           ["date"] = DateTime.TryParse(row.GetValueOrDefault("order_date", string.Empty)?.ToString(), out var orderDate) ? orderDate : (object)DBNull.Value,
    //           ["status"] = row.GetValueOrDefault("order_status", DBNull.Value),
    //           ["shipping_address"] = row.GetValueOrDefault("order_shipping_address", DBNull.Value),
    //           ["country"] = row.GetValueOrDefault("order_country", DBNull.Value),
    //           ["is_priority"] = row.GetValueOrDefault<bool>("order_is_priority", false)
    //           // ["is_priority"] = bool.TryParse(row.GetValueOrDefault("IsPriority", "false")?.ToString(), out var isPriority) && isPriority
    //         });
    //     yield break;
      // case "orderItem":
      //   yield return new TableTransformer(
      //       "_rawCsv",
      //       "OrderItem",
      //       row => new Dictionary<string, object?>
      //       {
      //         ["Id"] = row.GetValueOrDefault("Id", DBNull.Value),
      //         ["OrderId"] = row.GetValueOrDefault("OrderId", DBNull.Value),
      //         ["ProductId"] = row.GetValueOrDefault("ProductId", DBNull.Value),
      //         ["Quantity"] = int.TryParse(row.GetValueOrDefault("Quantity", "0")?.ToString(), out var quantity) ? quantity : 0,
      //         ["UnitPrice"] = decimal.TryParse(row.GetValueOrDefault("UnitPrice", "0")?.ToString(), out var unitPrice) ? unitPrice : 0m
      //       });
      //   yield break;
      // case "audit":
      //   yield return new TableTransformer(
      //       "_rawCsv",
      //       "Audit",
      //       row => new Dictionary<string, object?>
      //       {
      //         ["Id"] = row.GetValueOrDefault("AuditId", DBNull.Value),
      //         ["OrderId"] = row.GetValueOrDefault("OrderId", DBNull.Value),
      //         ["CustomerId"] = row.GetValueOrDefault("CustomerId", DBNull.Value),
      //         ["Action"] = row.GetValueOrDefault("Action", DBNull.Value),
      //         ["ActionDate"] = DateTime.TryParse(row.GetValueOrDefault("ActionDate", string.Empty)?.ToString(), out var dt) ? dt : DBNull.Value,
      //         ["Notes"] = row.GetValueOrDefault("Notes", DBNull.Value)
      //       });
      //   yield break;
      // default:
      //   throw new NotSupportedException($"Transform type '{context.Parameters["transform"]}' is not supported.");
    // }
  }
}
