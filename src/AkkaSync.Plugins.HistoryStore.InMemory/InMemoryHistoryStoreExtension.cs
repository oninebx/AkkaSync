using System;
using AkkaSync.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AkkaSync.Plugins.HistoryStore.InMemory;

public static class InMemoryHistoryStoreExtension
{
  public static IServiceCollection AddInMemoryHistoryStore(this IServiceCollection services)
  {
    services.AddSingleton<IHistoryStore, InMemoryHistoryStore>();
    services.AddSingleton<IPluginProvider<IHistoryStore>, InMemoryHistoryStoreProvider>();
    return services;
  }
}
