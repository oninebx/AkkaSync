using System;
using System.Collections.Frozen;
using System.Collections.ObjectModel;
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

  public IReadOnlyDictionary<string, IReadOnlySet<string>> FileEntries => _providers.GroupBy(kv => kv.Value.GetType().Assembly.GetName().Name)
      .ToDictionary(g => g.Key, g => (IReadOnlySet<string>)new HashSet<string>(g.Select(kv => kv.Key)));

  public bool AddProvider(IPluginProvider<T> provider) => _providers.TryAdd(provider.Key, provider);

  public IPluginProvider<T>? GetProvider(string key)
  {
    _ = _providers.TryGetValue(key, out var provider);
    return provider;
  }

  public bool RemoveProvider(string key) => _providers.Remove(key);
}
