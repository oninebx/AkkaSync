using System;
using System.Threading.Tasks.Dataflow;
using AkkaSync.Core.Abstractions;

namespace AkkaSync.Core.Abstractions;

public interface IPluginProviderRegistry<T> where T : class
{
  IPluginProvider<T>? GetProvider(string key);
}


