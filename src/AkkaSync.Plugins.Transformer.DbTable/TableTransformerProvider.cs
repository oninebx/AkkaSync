using System;
using System.Text.Json;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Plugins.Transformer.DbTable;

public class TableTransformerProvider : IPluginProvider<ISyncTransformer>
{
  public string Key => nameof(TableTransformerProvider);

  public IEnumerable<ISyncTransformer> Create(PluginSpec context, CancellationToken cancellationToken = default)
  {
    var configFile = Path.Combine(AppContext.BaseDirectory, context.Parameters.Get<string>("transformers"));
    if (!File.Exists(configFile))
    {
      throw new FileNotFoundException($"TableTransformerProvider config file not found: {configFile}");
    }
    var json = File.ReadAllText(configFile);

    var specs = JsonSerializer.Deserialize<IReadOnlyList<TableTransformerSpec>>(json, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });
    if(specs == null || specs.Count == 0)
    {
      throw new InvalidOperationException("TableTransformerProvider found no transformer specs in config file.");
    }
    foreach (var spec in specs)
    {
      yield return new TableTransformer(
        spec.Name,
        spec.Produce,
        spec.DependsOn,
        row =>
        {
          var mapped = new Dictionary<string, object?>();
          for (int i = 0; i < spec.Fields.Length && i < spec.Columns.Length; i++)
          {
            var field = spec.Fields[i];
            var column = spec.Columns[i];
            if (row.TryGetValue(field, out var value))
            {
              mapped[column] = value;
            }
            else
            {
              mapped[column] = null;
            }
          }
          return mapped;
        });
    }
  }
}
