using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks.Dataflow;

namespace AkkaSync.Abstractions;

public interface IPluginProviderRegistry<T> where T : class
{
  IPluginProvider<T>? GetProvider(string key);
  bool AddProvider(IPluginProvider<T> provider);
  int Count { get; }
  bool RemoveProvider(string key);

  IReadOnlyDictionary<string, IReadOnlySet<string>> FileEntries { get; }
}


