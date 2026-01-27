using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace AkkaSync.DataGenerator.FileGenerator;

public class CsvFileGenerator
{
  public static void GenerateOrdersCsv(string outputDir)
  {
    const int MaxCustomers = 2_000;
    const int TotalRows = 12_500;
    const int MinBatchSize = 200;
    const int MaxBatchSize = 800;

    var random = new Random(42);

    Directory.CreateDirectory(outputDir);

    var header = string.Join(",",
        "customer_external_id",
        "customer_first_name",
        "customer_last_name",
        "customer_email",
        "customer_phone",
        "customer_dob",
        "customer_country",
        "customer_is_active",
        "order_external_id",
        "order_date",
        "order_status",
        "order_shipping_address",
        "order_country",
        "order_is_priority",
        "product_sku",
        "product_name",
        "product_price",
        "order_item_quantity",
        "audit_action",
        "audit_notes"
    );

    var customers = Enumerable.Range(1, MaxCustomers)
        .Select(c => new
        {
          Id = $"CUST-{c:D6}",
          FirstName = $"First{c}",
          LastName = $"Last{c}",
          Email = $"user{c}@example.com",
          Phone = $"+64{random.Next(200000000, 299999999)}",
          Dob = RandomDate(random, 1965, 2002),
          Country = "NZ",
          IsActive = 1
        })
        .ToArray();

    int currentRow = 0;
    int fileNumber = 1;
    int orderSequence = 1;

    while (currentRow < TotalRows)
    {
      var batchSize = random.Next(MinBatchSize, MaxBatchSize + 1);
      batchSize = Math.Min(batchSize, TotalRows - currentRow);

      var outputPath = Path.Combine(
          outputDir,
          $"orders_import_{fileNumber:D3}.csv"
      );

      using var writer = new StreamWriter(
          outputPath,
          false,
          Encoding.UTF8,
          bufferSize: 1024 * 1024
      );

      writer.WriteLine(header);

      int writtenInFile = 0;

      while (writtenInFile < batchSize)
      {
        var customer = customers[random.Next(customers.Length)];

        var ordersPerCustomer = random.Next(1, 4);

        for (int o = 0; o < ordersPerCustomer && writtenInFile < batchSize; o++)
        {
          var orderId = $"ORD-{orderSequence++:D8}";
          var orderDate = RandomDate(random, 2023, 2025);
          var status = RandomFrom(random, "CREATED", "PAID", "SHIPPED");
          var address = $"#{random.Next(1, 200)} Queen St, Auckland";
          var isPriority = random.Next(0, 10) == 0 ? 1 : 0;

          var itemsPerOrder = random.Next(1, 6);

          for (int i = 0; i < itemsPerOrder && writtenInFile < batchSize; i++)
          {
            var productId = random.Next(1, 50_000);
            var sku = $"SKU-{productId:D5}";
            var productName = $"Product-{productId}";
            var price = Math.Round(random.NextDouble() * 500 + 10, 2);
            var quantity = random.Next(1, 5);

            writer.WriteLine(string.Join(",",
                customer.Id,
                customer.FirstName,
                customer.LastName,
                customer.Email,
                customer.Phone,
                customer.Dob,
                customer.Country,
                customer.IsActive,
                orderId,
                orderDate,
                status,
                $"\"{address}\"",
                customer.Country,
                isPriority,
                sku,
                productName,
                price.ToString("F2", CultureInfo.InvariantCulture),
                quantity,
                "ORDER_IMPORTED",
                "Bulk CSV import"
            ));

            writtenInFile++;
            currentRow++;
          }
        }
      }

      Console.WriteLine(
          $"Generated file {fileNumber:D3}: {writtenInFile:N0} rows (total {currentRow:N0})"
      );

      fileNumber++;
    }

    Console.WriteLine("Orders CSV generation completed.");
  }


  public static void GenerateSystemEventsCsv(string outputDir)
  {
    const int TotalEvents = 10_000;
    const int MinBatchSize = 400;
    const int MaxBatchSize = 1600;

    var random = new Random(42);

    Directory.CreateDirectory(outputDir);

    var header = string.Join(",",
        "event_id",
        "occurred_at",
        "system_id",
        "system_type",
        "component",
        "host",
        "metric_name",
        "metric_value",
        "metric_unit",
        "severity",
        "reported_at"
    );

    int eventSequence = 1;
    int fileNumber = 1;
    int generated = 0;

    while (generated < TotalEvents)
    {
      var batchSize = random.Next(MinBatchSize, MaxBatchSize + 1);
      batchSize = Math.Min(batchSize, TotalEvents - generated);

      var outputPath = Path.Combine(
          outputDir,
          $"system_events_{fileNumber:D3}.csv"
      );

      using var writer = new StreamWriter(
          outputPath,
          false,
          Encoding.UTF8,
          bufferSize: 1024 * 1024
      );

      writer.WriteLine(header);

      for (int i = 0; i < batchSize; i++)
      {
        var systemIndex = random.Next(1, 501);
        var systemId = $"SYS-{systemIndex:D4}";
        var systemType = RandomFrom(random, "api", "worker", "db", "cache");
        var component = RandomFrom(random, "auth", "sync", "ingest", "dispatcher");
        var host = $"node-{random.Next(1, 50):D2}";

        var metricName = RandomFrom(
            random,
            "cpu_usage",
            "mem_usage",
            "queue_depth",
            "latency_ms"
        );

        var (metricValue, metricUnit) = metricName switch
        {
          "cpu_usage" => (Math.Round(random.NextDouble() * 90 + 5, 2), "%"),
          "mem_usage" => (Math.Round(random.NextDouble() * 32000 + 512, 0), "MB"),
          "queue_depth" => (random.Next(0, 5000), "count"),
          "latency_ms" => (Math.Round(random.NextDouble() * 1500 + 10, 1), "ms"),
          _ => (0.0, "")
        };

        var severity = metricName switch
        {
          "cpu_usage" when metricValue > 85 => "ERROR",
          "cpu_usage" when metricValue > 70 => "WARN",
          "latency_ms" when metricValue > 1200 => "ERROR",
          "latency_ms" when metricValue > 800 => "WARN",
          _ => "INFO"
        };

        var occurredAt = RandomRecentDate(random, daysBack: 7);
        var reportedAt = occurredAt.AddSeconds(random.Next(0, 30));

        writer.WriteLine(string.Join(",",
            $"EVT-{eventSequence++:D9}",
            occurredAt.ToString("O"),
            systemId,
            systemType,
            component,
            host,
            metricName,
            metricValue.ToString(CultureInfo.InvariantCulture),
            metricUnit,
            severity,
            reportedAt.ToString("O")
        ));
      }

      generated += batchSize;

      Console.WriteLine(
          $"Generated file {fileNumber:D3}: {batchSize:N0} rows (total {generated:N0})"
      );

      fileNumber++;
    }

    Console.WriteLine("System events CSV generation completed.");
  }


  static string RandomDate(Random random, int startYear, int endYear)
  {
    var start = new DateTime(startYear, 1, 1);
    var range = (new DateTime(endYear, 12, 31) - start).Days;
    return start.AddDays(random.Next(range)).ToString("yyyy-MM-dd");
  }

  static string RandomFrom(Random random, params string[] values)
      => values[random.Next(values.Length)];

  static DateTime RandomRecentDate(Random random, int daysBack)
  {
    var now = DateTime.UtcNow;
    var seconds = random.Next(0, daysBack * 24 * 3600);
    return now.AddSeconds(-seconds);
  }
}

