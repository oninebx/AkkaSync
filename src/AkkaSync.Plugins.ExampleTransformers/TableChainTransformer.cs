using System;
using System.Runtime.CompilerServices;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Plugins.ExampleTransformers;

public class TableChainTransformer : TransformerBase
{
  private readonly string _sourceTable;
  private readonly string _targetTable;
  private readonly Func<Dictionary<string, object?>, Dictionary<string, object?>> _mapRow;

  public TableChainTransformer(
    string sourceTable,
    string targetTable,
    Func<Dictionary<string, object?>, Dictionary<string, object?>> mapRow)
  {
    _sourceTable = sourceTable;
    _targetTable = targetTable;
    _mapRow = mapRow;
  }

  protected override TransformContext Process(TransformContext context)
  {
    // if (!context.TablesData.TryGetValue(_sourceTable, out var tableData))
    //     throw new InvalidOperationException($"Source table '{_sourceTable}' not found.");

    // var mappedRow = _mapRow(tableData);
    // context.TablesData[_targetTable] = mappedRow;

    // context.TablesData.Remove(_sourceTable);

    return context;
  }
}
