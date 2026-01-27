
using AkkaSync.DataGenerator.FileGenerator;


var examplesRoot = GetExamplesRoot();
var ordersDir = Path.Combine(examplesRoot, "data", "csv", "orders");
var systemEventsDir = Path.Combine(examplesRoot, "data", "csv", "system_events");

RecreateDirectories(ordersDir, systemEventsDir);

CsvFileGenerator.GenerateOrdersCsv(ordersDir);
CsvFileGenerator.GenerateSystemEventsCsv(systemEventsDir);

static string GetExamplesRoot()
{
  var dir = AppContext.BaseDirectory;

  while (dir != null)
  {
    var exampleDir = Path.Combine(dir, "examples");
    if (Directory.Exists(exampleDir))
      return exampleDir;

    dir = Directory.GetParent(dir)?.FullName;
  }

  throw new InvalidOperationException("examples root not found.");
}

static void RecreateDirectories(params string[] directories)
{
  foreach (var directory in directories)
  {
    if (string.IsNullOrWhiteSpace(directory))
      continue;

    // 可选：安全防线
    if (!directory.Contains(Path.Combine("data", "csv")))
    {
      throw new InvalidOperationException(
          $"Refusing to delete non-csv directory: {directory}"
      );
    }

    if (Directory.Exists(directory))
    {
      Directory.Delete(directory, recursive: true);
      Console.WriteLine($"Deleted directory: {directory}");
    }

    Directory.CreateDirectory(directory);
    Console.WriteLine($"Created directory: {directory}");
  }
}
