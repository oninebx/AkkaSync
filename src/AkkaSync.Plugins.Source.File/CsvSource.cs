using System;
using System.Runtime.CompilerServices;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace AkkaSync.Plugins.Sources;

public class CsvSource : ISyncSource
{

  private readonly string _filePath;
  private readonly char _delimiter;
  private readonly ILogger<CsvSource> _logger;
  private readonly Lazy<string> _id;
  private readonly Lazy<string> _etag;

  public CsvSource(string filePath, ISyncEnvironment environment, ILogger<CsvSource> logger, char delimiter = ',')
  {
    _filePath = filePath;
    _delimiter = delimiter;
    _logger = logger;

    _id = new Lazy<string>(() =>
    {
      return environment.ComputeSha256(Type, Key);
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    _etag = new Lazy<string>(() =>
    {
      var info = new FileInfo(_filePath);
      var input = $"{info.Length}:{info.LastWriteTimeUtc.Ticks}";

      return environment.ComputeSha256(info.Length.ToString(), info.LastWriteTimeUtc.Ticks.ToString());
    });
  }

  public string Key => Path.GetFullPath(_filePath).Replace('\\', '/').ToLowerInvariant();

  public string Type => "CSV";

  public string Id => _id.Value;

  public string ETag => _etag.Value;

  public async IAsyncEnumerable<TransformContext> ReadAsync(string? cursor, [EnumeratorCancellation] CancellationToken cancellationToken)
  {
    _ = int.TryParse(cursor, out int startRow);
    using var reader = new StreamReader(_filePath);
    string? headerLine = await reader.ReadLineAsync();
    if (headerLine == null)
    {
      yield break;
    }
    var headers = headerLine.Split(_delimiter);
    int lineNumber = 1;
    int index = 0;
    while (index < startRow && !reader.EndOfStream)
    {
      await reader.ReadLineAsync();
      index++;
    }
    while (!reader.EndOfStream && !cancellationToken.IsCancellationRequested)
    {
      var line = await reader.ReadLineAsync(cancellationToken);
      if (string.IsNullOrWhiteSpace(line))
      {
        continue;
      }
      lineNumber++;
      string[] values;
      try
      {
        values = ParseCsvLine(line, _delimiter);
        if (values.Length != headers.Length)
        {
          throw new FormatException("Column count does not match header");
        }
      }
      catch (FormatException ex)
      {
        _logger.LogWarning(ex, "CSV format error at line {Line} in file {File}", lineNumber, _filePath);
        continue;
      }
      var record = new Dictionary<string, object?>();
      for (int i = 0; i < headers.Length && i < values.Length; i++)
      {
        record[headers[i]] = values[i];
      }
      var ctx = new TransformContext(record)
      {
        MetaData = new Dictionary<string, object>
        {
          ["SourceType"] = "CSV",
          ["FilePath"] = _filePath,
          ["LineNumber"] = lineNumber,
        },
        Cursor = index.ToString()
      };

      yield return ctx;
      index++;
    }
  }

  /// <summary>
  /// Parse a CSV line handling quoted fields correctly
  /// </summary>
  private string[] ParseCsvLine(string line, char delimiter)
  {
    var fields = new List<string>();
    var currentField = new System.Text.StringBuilder();
    bool insideQuotes = false;

    for (int i = 0; i < line.Length; i++)
    {
      char c = line[i];

      if (c == '"')
      {
        // Check for escaped quote (double quote)
        if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
        {
          currentField.Append('"');
          i++; // Skip the next quote
        }
        else
        {
          insideQuotes = !insideQuotes;
        }
      }
      else if (c == delimiter && !insideQuotes)
      {
        fields.Add(currentField.ToString());
        currentField.Clear();
      }
      else
      {
        currentField.Append(c);
      }
    }

    // Add the last field
    fields.Add(currentField.ToString());

    return [.. fields];
  }
}
