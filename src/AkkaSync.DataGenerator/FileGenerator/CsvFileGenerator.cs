using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace AkkaSync.DataGenerator.FileGenerator;

public class CsvFileGenerator
{
  public static void GenerateOrdersCsv()
  {
    const int CustomerCount = 500_000;
        const int OrdersPerCustomer = 2;
        const int ItemsPerOrder = 3;

        var random = new Random(42);
        var solutionRoot = GetSolutionRoot();

        var outputDir = Path.Combine(
            solutionRoot,
            "Samples",
            "inputs",
            "csv"
        );

Directory.CreateDirectory(outputDir);

var outputPath = Path.Combine(outputDir, "orders_import.csv");

        using var writer = new StreamWriter(outputPath, false, Encoding.UTF8, bufferSize: 1024 * 1024);

        // Header
        writer.WriteLine(string.Join(",",
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
        ));

        int orderSequence = 1;

        for (int c = 1; c <= CustomerCount; c++)
        {
            var customerId = $"CUST-{c:D6}";
            var firstName = $"First{c}";
            var lastName = $"Last{c}";
            var email = $"user{c}@example.com";
            var phone = $"+64{random.Next(200000000, 299999999)}";
            var dob = RandomDate(random, 1965, 2002);
            var country = "NZ";
            var isActive = 1;

            for (int o = 0; o < OrdersPerCustomer; o++)
            {
                var orderId = $"ORD-{orderSequence++:D8}";
                var orderDate = RandomDate(random, 2023, 2025);
                var status = RandomFrom(random, "CREATED", "PAID", "SHIPPED");
                var address = $"#{random.Next(1, 200)} Queen St, Auckland";
                var isPriority = random.Next(0, 10) == 0 ? 1 : 0;

                for (int i = 0; i < ItemsPerOrder; i++)
                {
                    var productId = random.Next(1, 50_000);
                    var sku = $"SKU-{productId:D5}";
                    var productName = $"Product-{productId}";
                    var price = Math.Round(random.NextDouble() * 500 + 10, 2);
                    var quantity = random.Next(1, 5);

                    var auditAction = "ORDER_IMPORTED";
                    var auditNotes = "Bulk CSV import";

                    writer.WriteLine(string.Join(",",
                        customerId,
                        firstName,
                        lastName,
                        email,
                        phone,
                        dob,
                        country,
                        isActive,
                        orderId,
                        orderDate,
                        status,
                        $"\"{address}\"",
                        country,
                        isPriority,
                        sku,
                        productName,
                        price.ToString("F2", CultureInfo.InvariantCulture),
                        quantity,
                        auditAction,
                        auditNotes
                    ));
                }
            }

            // Progress log
            if (c % 10_000 == 0)
            {
                Console.WriteLine($"Generated customer {c:N0}");
            }
        }

        Console.WriteLine("CSV generation completed.");
    
  }

  static string RandomDate(Random random, int startYear, int endYear)
    {
        var start = new DateTime(startYear, 1, 1);
        var range = (new DateTime(endYear, 12, 31) - start).Days;
        return start.AddDays(random.Next(range)).ToString("yyyy-MM-dd");
    }

    static string RandomFrom(Random random, params string[] values)
        => values[random.Next(values.Length)];
    
    public static string GetSolutionRoot()
    {
        var dir = AppContext.BaseDirectory;

        while (dir != null)
        {
            if (File.Exists(Path.Combine(dir, "AkkaSync.sln")))
                return dir;

            dir = Directory.GetParent(dir)?.FullName;
        }

        throw new InvalidOperationException("Solution root not found.");
    }
}

