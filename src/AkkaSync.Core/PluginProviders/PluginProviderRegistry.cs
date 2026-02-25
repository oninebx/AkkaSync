using System;
using System.Collections.Frozen;
using System.Collections.ObjectModel;
using System.Text.Json;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Core.PluginProviders;

public class PluginProviderRegistry<T> : IPluginProviderRegistry<T> where T : class
{
  private readonly Dictionary<string, IPluginProvider<T>> _providers;

  public PluginProviderRegistry(IEnumerable<IPluginProvider<T>> providers)
  {
    _providers = providers.ToDictionary(p => p.Key, p => p);
  }

  public int Count => _providers.Count;

  public IReadOnlyDictionary<string, IReadOnlySet<string>> FileEntries => _providers.GroupBy(kv => kv.Value.GetType().Assembly.GetName().Name!)
      .ToDictionary(g => g.Key, g => (IReadOnlySet<string>)new HashSet<string>(g.Select(kv => kv.Key)));

  public bool AddProvider(IPluginProvider<T> provider) => _providers.TryAdd(provider.Key, provider);

  public IPluginProvider<T>? GetProvider(string key)
  {
    _ = _providers.TryGetValue(key, out var provider);
    return provider;
  }

  public bool TryRemoveProvider(string key, out PluginDescriptor descriptor) 
  {
    if(!_providers.TryGetValue(key, out var provider) || !_providers.Remove(key))
    {
      descriptor = default!;
      return false;
    }
    
    descriptor = new PluginDescriptor(provider.Key, provider.Version.ToString());
    return true;
  }

  public IEnumerable<PluginDescriptor> ToDescriptors()
  {
    var descriptors = _providers.Select(p =>new PluginDescriptor(Name: p.Key, Version: p.Value.Version.ToString()));
    return descriptors;
  }
}
