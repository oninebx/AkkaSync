using System;
using System.Threading.Tasks.Dataflow;

namespace AkkaSync.Abstractions;

public interface IPluginProviderRegistry<T> where T : class
{
  IPluginProvider<T>? GetProvider(string key);
  bool AddProvider(IPluginProvider<T> provider);
  int Count { get; }
}


