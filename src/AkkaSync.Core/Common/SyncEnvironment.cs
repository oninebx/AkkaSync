using System;
using System.Security.Cryptography;
using System.Text;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Common;

public class SyncEnvironment : ISyncEnvironment
{
  private readonly string _runtimeRoot;
  private SyncEnvironment(string runtimeRoot)
  {
    _runtimeRoot = runtimeRoot;
  }
  public string ComputeSha256(params string[] values)
  {
    if (values == null || values.Length == 0)
    {
      throw new ArgumentException("At least one string must be provided", nameof(values));
    }


    var normalized = values
        .Where(v => !string.IsNullOrWhiteSpace(v))
        .Select(v => v.Trim());

    var input = string.Join(",", normalized);
    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
    return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
  }

  public string ResolveConnectionString(string connStr)
  {
    const string prefix = "Data Source=";
    if (connStr.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
    {
      var pathPart = connStr[prefix.Length..].Trim();
      var resolvedPath = ResolveDataPath(pathPart);
      return $"{prefix}{resolvedPath}{connStr[(prefix.Length + pathPart.Length)..]}";
    }

    return connStr;
  }

  public string ResolveDataPath(string logicalPath)
  {
    if (string.IsNullOrWhiteSpace(_runtimeRoot))
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
      var combined = Path.Combine(_runtimeRoot, logicalPath);
      var fullPath = Path.GetFullPath(combined);

      if(!fullPath.StartsWith(_runtimeRoot, StringComparison.OrdinalIgnoreCase))
      {
        throw new InvalidOperationException($"Resolved path escapes DataRoot: {fullPath}");
      }
      return fullPath;
  }

  public static ISyncEnvironment Default()
  {
    return new SyncEnvironment(Path.Combine(AppContext.BaseDirectory, "data"));
  }

  public static ISyncEnvironment CreateDocker() => new SyncEnvironment("/app/data");
}
