using System;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace AkkaSync.Plugins.Sink.Sqlite;

public class SqliteSinkProvider : IPluginProvider<ISyncSink>
{
  private readonly ILoggerFactory _factory;
  public string Key => nameof(SqliteSinkProvider);

  public SqliteSinkProvider(ILoggerFactory factory)
  {
    _factory = factory;
  }

  public IEnumerable<ISyncSink> Create(PluginSpec context, CancellationToken cancellationToken = default)
  {
    var connectionString = context.Parameters["connectionString"];

    yield return new SqliteSink(connectionString, _factory.CreateLogger<SqliteSink>());
  }
}
