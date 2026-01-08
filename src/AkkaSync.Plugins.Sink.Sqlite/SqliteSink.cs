using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using SQLitePCL;

namespace AkkaSync.Plugins.Sink.Sqlite
{
  public class SqliteSink : ISyncSink
  {

    private readonly string _connectionString;
    private static readonly SemaphoreSlim _writeLock = new(1, 1);
    private readonly ILogger<SqliteSink> _logger;
    private static ImmutableHashSet<int> RECOVERABLE_ERROR_CODE =
    [
      raw.SQLITE_CONSTRAINT,
      raw.SQLITE_CONSTRAINT_NOTNULL,
      raw.SQLITE_CONSTRAINT_PRIMARYKEY,
      raw.SQLITE_CONSTRAINT_UNIQUE,
      raw.SQLITE_CONSTRAINT_FOREIGNKEY
    ];

    public SqliteSink(string connectionString, ILogger<SqliteSink> logger)
    {
      _connectionString = connectionString;
      _logger = logger;
    }
    public async Task WriteAsync(IEnumerable<TransformContext> contextBatch, CancellationToken cancellationToken)
    {
      
      if (contextBatch == null || !contextBatch.Any())
      {
        return;
      }
      await _writeLock.WaitAsync(cancellationToken);
      string tableName = string.Empty;
      try
      {
        using var connection = new SqliteConnection(_connectionString);
        
        await connection.OpenAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        var tables = contextBatch.Where(ctx => ctx?.TablesData?.Count > 0)
                            .SelectMany(ctx => ctx.TablesData)
                            .GroupBy(t => t.Key);
        foreach(var table in tables)
        {
          tableName = table.Key;
          var rows = table.Select(t => t.Value).Where(r => r != null && r.Count > 0);
          await InsertTableDataAsync(tableName, rows, connection, transaction, cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
        _logger.LogInformation("Transaction committed successfully.");
      }
      catch(Exception ex)
        {
          _logger.LogError(ex, "Fatal error occurred while inserting rows into table {TableName}.", tableName);
          throw;
        }
      finally
      {
        _writeLock.Release();
      }
      
    }

    private async Task InsertTableDataAsync(
      string table, 
      IEnumerable<Dictionary<string, object?>> rows, 
      SqliteConnection connection, 
      SqliteTransaction transaction, 
      CancellationToken cancellationToken)
    {
      if(!rows.Any())
      {
        _logger.LogInformation("Table {TableName} has no data, skipping.", table);
        return;
      }

      string Escape(string name) => $"\"{name.Replace("\"", "\"\"")}\"";
      var firstRow = rows.First();
      var tableName = Escape(table);
      var columns = firstRow.Keys.ToList();
      var columnNames = string.Join(", ", columns.Select(Escape));
      var parameterNames = string.Join(", ", columns.Select(c => "@" + c));
      var insertStatement = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames})";
      
      await using var cmd = new SqliteCommand(insertStatement, connection, transaction);
      foreach(var column in columns)
      {
        cmd.Parameters.Add(new SqliteParameter($"@{column}", DBNull.Value));
      }

      foreach(var row in rows)
      {
        foreach(var column in columns)
        {
          cmd.Parameters[$"@{column}"].Value = row[column] ?? DBNull.Value;
        }
        try
        {
          await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
        catch(SqliteException ex) when (RECOVERABLE_ERROR_CODE.Contains(ex.SqliteErrorCode))
        {
          _logger.LogWarning(ex, "Recoverable error inserting row into {TableName}, skipping row.", tableName);
          continue;
        }
      }
      _logger.LogInformation("Finished inserting rows into table {TableName}.", tableName);
    }
  }
}
