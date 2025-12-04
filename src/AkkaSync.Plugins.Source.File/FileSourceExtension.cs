using System;
using AkkaSync.Abstractions;
using AkkaSync.Plugins.Sources;
using Microsoft.Extensions.DependencyInjection;

namespace AkkaSync.Plugins.Source.File;

public static class FileSourceExtension
{
  public static IServiceCollection AddFileSource(this IServiceCollection services)
  {
    services.AddSingleton<ISyncSource, CsvSource>();
    services.AddSingleton<IPluginProvider<ISyncSource>, FolderWatcherSourceProvider>();
    return services;
  }
}
