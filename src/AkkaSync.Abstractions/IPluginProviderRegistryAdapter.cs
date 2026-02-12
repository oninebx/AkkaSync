using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Abstractions
{
  public interface IPluginProviderRegistryAdapter
  {
    bool CanHandle(Type pluginType);
    void AddProvider(object provider);
    bool RemoveByFile(string filePath);
    int Count { get; }
  }
}
