using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using AkkaSync.Core.Abstractions;

namespace AkkaSync.Core.PluginProviders;

public class HistoryStoreRegistry : IPluginProviderRegistry<IHistoryStore>
{
  public IPluginProvider<IHistoryStore>? GetProvider(string key)
  {
    throw new NotImplementedException();
  }
}
