using System;
using AkkaSync.Core.Configuration;
using AkkaSync.Core.Pipeline;
using AkkaSync.Core.PluginProviders;
using AkkaSync.Plugins.Sinks.Factories;

namespace AkkaSync.Plugins.Sinks;

public class DatabaseSinkProvider : IPluginProvider<ISyncSink>
{
  private readonly IEnumerable<IDatabaseSinkFactory> _factories;
  public string Key => nameof(DatabaseSinkProvider);

  public DatabaseSinkProvider(IEnumerable<IDatabaseSinkFactory> factories)
  {
    _factories = factories;
  }

  public IEnumerable<ISyncSink> Create(PipelineContext context, CancellationToken cancellationToken)
  {
    var sink = context.SinkProvider.Parameters["sink"];
    var connectionString = context.SinkProvider.Parameters["connectionString"];
    var factory = _factories.First(f => f.CanCreate(sink));

    yield return factory.Create(connectionString);
  }
}
    