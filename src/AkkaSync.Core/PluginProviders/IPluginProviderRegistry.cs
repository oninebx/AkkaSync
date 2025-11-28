using System;
using System.Threading.Tasks.Dataflow;
using AkkaSync.Core.Pipeline;

namespace AkkaSync.Core.PluginProviders;

public interface IPluginProviderRegistry<T> where T : class
{
  IPluginProvider<T>? GetProvider(string key);
}


