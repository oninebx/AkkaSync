using AkkaSync.Abstractions.Models;
using System;

namespace AkkaSync.Abstractions;

public interface IPluginProviderRegistry<T> where T : class
{
  IPluginProvider<T>? GetProvider(string key);
  bool AddProvider(IPluginProvider<T> provider);
  int Count { get; }
  bool TryRemoveProvider(string key, out PluginDescriptor descriptor);
  IEnumerable<PluginDescriptor> ToDescriptors();

  IReadOnlyDictionary<string, IReadOnlySet<string>> FileEntries { get; }
}


