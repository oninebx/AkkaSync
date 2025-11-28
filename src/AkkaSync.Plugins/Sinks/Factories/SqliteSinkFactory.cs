using System;
using AkkaSync.Core.Pipeline;

namespace AkkaSync.Plugins.Sinks.Factories;

public class SqliteSinkFactory : IDatabaseSinkFactory
{
  public bool CanCreate(string type) => type.Equals("sqlite", StringComparison.OrdinalIgnoreCase);

  public ISyncSink Create(string connectionString) => new SqliteSink(connectionString);
}
