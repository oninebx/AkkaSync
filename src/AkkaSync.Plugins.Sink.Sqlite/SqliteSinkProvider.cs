using System;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace AkkaSync.Plugins.Sink.Sqlite;

public class SqliteSinkProvider : IPluginProvider<ISyncSink>
{
  private readonly ILoggerFactory _factory;
  private readonly ISyncEnvironment _environment;
  public string Key => nameof(SqliteSinkProvider);

  public SqliteSinkProvider(ISyncEnvironment environment, ILoggerFactory factory)
  {
    _factory = factory;
    _environment = environment;
  }

  public IEnumerable<ISyncSink> Create(PluginSpec context, CancellationToken cancellationToken = default)
  {
    var connectionString = _environment.ResolveConnectionString(context.Parameters["connectionString"]);

    yield return new SqliteSink(connectionString, _factory.CreateLogger<SqliteSink>());
  }
}
