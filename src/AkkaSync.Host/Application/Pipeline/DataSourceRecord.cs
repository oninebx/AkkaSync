using AkkaSync.Abstractions.Models;
using System.Text.RegularExpressions;

namespace AkkaSync.Host.Application.Pipeline
{
  public sealed record DataSourceRecord(
    string Id,
    string Name,
    string Type)
  {
    public static DataSourceRecord From(DataSourceMeta? meta)
    {
      return new DataSourceRecord(
          Id: CreateId(meta?.Type ?? "unknown", meta?.Name ?? "unknown"),
          Name: meta?.Name ?? "unknown",
          Type: meta?.Type ?? "unknown"
      );
    }

    private static string CreateId(string type, string name)
    {
      //var canonical = Canonicalize(type, value);

      return $"{Normalize(type)}:{Normalize(name)}";
    }

    private static string Normalize(string input)
    {
      input = input.Trim().ToLowerInvariant();

      input = Regex.Replace(input, @"[^a-z0-9]+", "-");

      input = input.Trim('-');

      return input;
    }

    //private static string Canonicalize(string type, string value)
    //{
    //  value = value.Trim().ToLowerInvariant();

    //  return type switch
    //  {
    //    "folder" or "file" => Path.GetFullPath(value),
    //    "sqlite" => CanonicalizeSqlite(value),
    //    _ => value
    //  };
    //}

    //private static string CanonicalizeSqlite(string value)
    //{
    //  value = value.Trim();

    //  if (value.Equals(":memory:", StringComparison.OrdinalIgnoreCase))
    //    return "sqlite::memory:";

    //  if (value.Contains("Data Source=", StringComparison.OrdinalIgnoreCase))
    //    value = value.Split(';')[0].Split('=')[1];

    //  var path = Path.GetFullPath(value)
    //      .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    //  return path;
    //}

  }
}
