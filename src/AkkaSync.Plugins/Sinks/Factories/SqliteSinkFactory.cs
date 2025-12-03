using System;
using AkkaSync.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace AkkaSync.Plugins.Sinks.Factories;

public class SqliteSinkFactory : IDatabaseSinkFactory
{
  private readonly ILoggerFactory _factory;
  public SqliteSinkFactory(ILoggerFactory loggerFactory)
  {
    _factory = loggerFactory;
  }
  public bool CanCreate(string type) => type.Equals("sqlite", StringComparison.OrdinalIgnoreCase);

  public ISyncSink Create(string connectionString) => new SqliteSink(connectionString, _factory.CreateLogger<SqliteSink>());
}
