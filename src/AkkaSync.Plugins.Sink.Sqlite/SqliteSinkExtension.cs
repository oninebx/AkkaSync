using System;
using AkkaSync.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AkkaSync.Plugins.Sink.Sqlite;

public static class SqliteSinkModule
{
  public static IServiceCollection AddSqliteSink(this IServiceCollection services)
  {
    services.AddSingleton<IPluginProvider<ISyncSink>, SqliteSinkProvider>();
    return services;
  }
}
