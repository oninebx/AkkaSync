using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkkaSync.Host.Infrastructure
{
  public static class DataPath
  {
    public static string DataRoot { get; private set; } = default!;

    public static void Initialize(string? overrideRoot = null)
    {
      DataRoot = ResolveDataRoot(overrideRoot);
    }

    public static string Resolve(string logicalPath)
    {
      if (string.IsNullOrWhiteSpace(DataRoot))
      {
        throw new InvalidOperationException("DataPath is not initialized. Call DataPath.Initialize() at host startup.");
      }
      if (Path.IsPathRooted(logicalPath))
      {
        throw new InvalidOperationException($"Absolute paths are not allowed: {logicalPath}");
      }
      if (logicalPath.Contains(".."))
      {
        throw new InvalidOperationException($"Parent path traversal is not allowed: {logicalPath}");
      }
      var combined = Path.Combine(DataRoot, logicalPath);
      var fullPath = Path.GetFullPath(combined);

      if(!fullPath.StartsWith(DataRoot, StringComparison.OrdinalIgnoreCase))
      {
        throw new InvalidOperationException($"Resolved path escapes DataRoot: {fullPath}");
      }
      return fullPath;
    }

    public static string ResolveDataRoot(string? overrideRoot)
    {
      if (!string.IsNullOrWhiteSpace(overrideRoot))
      {
        return Path.GetFullPath(overrideRoot);
      }
      // environment
      var env = Environment.GetEnvironmentVariable("AKKASYNC_DATA_ROOT");
      if (!string.IsNullOrWhiteSpace(env))
      {
        return Path.GetFullPath(env);
      }

      // local
      var dir = new DirectoryInfo(AppContext.BaseDirectory);
      while (dir != null)
      {
        var samplesPath = Path.Combine(dir.FullName, "samples");
        if (Directory.Exists(samplesPath))
        {
          return Path.GetFullPath(samplesPath);
        }
        dir = dir.Parent;
      }

      // fallback
      return Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "samples"));
    }
  }
}