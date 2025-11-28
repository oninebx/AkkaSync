using System;
using System.Runtime.CompilerServices;
using AkkaSync.Core.Pipeline;

namespace AkkaSync.Plugins.Sources;

public class CsvSource : ISyncSource
{

  private readonly string _filePath;
  private readonly char _delimiter;
  public CsvSource(string filePath, char delimiter = ',')
  {
    _filePath = filePath;
    _delimiter = delimiter;
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
      var line = await reader.ReadLineAsync();
      if (line == null)
      {
        continue;
      }
      lineNumber++;
      var values = line.Split(_delimiter);
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
