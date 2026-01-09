using System;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Plugins.Transformer.DbTable;

public class TableTransformer : ISyncTransformer
{
  public string Produce { get; init; }
  public string[] DependsOn { get; init; }
  private readonly string _table;
  private readonly Func<IReadOnlyDictionary<string, object?>, Dictionary<string, object?>> _mapRow;

  public TableTransformer(
    string table,
    string produce, 
    string[] dependsOn,
    Func<IReadOnlyDictionary<string, object?>, Dictionary<string, object?>> mapRow)
  {
    Produce = produce;
    DependsOn = dependsOn;
    _mapRow = mapRow;
    _table = table;
  }

  public Task<TransformContext> Transform(TransformContext context, CancellationToken cancellationToken)
  {
    var mappedRow = _mapRow(context.RawData);
    context.TryProduce(_table, mappedRow);
    return Task.FromResult(context);
  }
}

