using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.PluginProviders;

public class PluginProviderRegistry<T> : IPluginProviderRegistry<T> where T : class
{
  private readonly Dictionary<string, IPluginProvider<T>> _providers;

  public PluginProviderRegistry(IEnumerable<IPluginProvider<T>> providers)
  {
    _providers = providers.ToDictionary(p => p.Key, p => p);
  }

  public int Count => _providers.Count;

  public bool AddProvider(IPluginProvider<T> provider) => _providers.TryAdd(provider.Key, provider);

  public IPluginProvider<T>? GetProvider(string key)
  {
    _ = _providers.TryGetValue(key, out var provider);
    return provider;
  }
}
