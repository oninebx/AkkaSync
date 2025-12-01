using System;
using System.Runtime.CompilerServices;
using Akka.Event;
using AkkaSync.Core.Pipeline;
using Microsoft.Extensions.Logging;

namespace AkkaSync.Plugins.Sources;

public class CsvSource : ISyncSource
{

  private readonly string _filePath;
  private readonly char _delimiter;
  private readonly ILogger<CsvSource> _logger;
  public CsvSource(string filePath, ILogger<CsvSource> logger, char delimiter = ',')
  {
    _filePath = filePath;
    _delimiter = delimiter;
    _logger = logger;
  }

  public string Key => $"_csv_{_filePath.GetHashCode()}";

  public async IAsyncEnumerable<TransformContext> ReadAsync([EnumeratorCancellation] CancellationToken cancellationToken)
  {
    using var reader = new StreamReader(_filePath);
    string? headerLine = await reader.ReadLineAsync();
    if (headerLine == null)
    {
      yield break;
    }
    var headers = headerLine.Split(_delimiter);
    int lineNumber = 1;
    while (!reader.EndOfStream)
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
        values = line.Split(_delimiter);
        if(values.Length != headers.Length)
        {
          throw new FormatException("Column count does not match header");
        }
      }
      catch (FormatException ex)
      {
        _logger.LogWarning(ex, "CSV format error at line {Line} in file {File}", lineNumber, _filePath);
        continue;
      }
      var record = new Dictionary<string, object>();
      for (int i = 0; i < headers.Length && i < values.Length; i++)
      {
        record[headers[i]] = values[i];
      }
      var ctx = new TransformContext
        {
            RawData = line,
            TablesData = new Dictionary<string, Dictionary<string, object>>
            {
                ["_rawCsv"] = record
            },
            MetaData = new Dictionary<string, object>
            {
                ["SourceType"] = "CSV",
                ["FilePath"] = _filePath,
                ["LineNumber"] = lineNumber,
            }
        };

        yield return ctx;
    } 
  }
}
