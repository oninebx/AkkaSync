using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AkkaSync.Core.Pipeline;
using Microsoft.Data.Sqlite;

namespace AkkaSync.Plugins.Sinks
{
  public class SqliteSink : ISyncSink
  {

    private readonly string _connectionString;
    private static readonly SemaphoreSlim _writeLock = new(1, 1);
    private SqliteConnection? _connection;
    private SqliteTransaction? _transaction;

    public SqliteSink(string connectionString)
    {
      _connectionString = connectionString;
    }
    public async Task WriteAsync(IEnumerable<TransformContext> contextList, CancellationToken cancellationToken)
    {
      string Escape(string name) => $"\"{name.Replace("\"", "\"\"")}\"";
      if (contextList == null || !contextList.Any())
      {
        return;
      }
      var contexts = contextList.Where(ctx => ctx?.TablesData != null && ctx.TablesData.Count != 0).ToList();
      if(contexts.Count == 0)
      {
        return;
      }
      await _writeLock.WaitAsync(cancellationToken);
      try
      {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();
        foreach (var context in contexts)
        {
          foreach (var tableData in context.TablesData)
          {
            var tableName = Escape(tableData.Key);
            var row = tableData.Value;
            if (row == null || row.Count == 0)
            {
              continue;
            }
            var columns = row.Keys.ToList();
            var columnNames = string.Join(", ", columns.Select(Escape));
            var parameterNames = string.Join(", ", columns.Select(c => "@" + c));

            var insertCommandText = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames})";
            using var insertCommand = new SqliteCommand(insertCommandText, connection, transaction);
            foreach (var column in columns)
            {
              insertCommand.Parameters.AddWithValue("@" + column, row[column] ?? DBNull.Value);
            }
            await insertCommand.ExecuteNonQueryAsync(cancellationToken);
          }
        }
        await transaction.CommitAsync(cancellationToken);
      }
      finally
      {
        _writeLock.Release();
      }
    }
  }
}
