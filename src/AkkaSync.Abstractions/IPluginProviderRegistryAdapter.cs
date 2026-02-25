using AkkaSync.Abstractions.Models;
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
    PluginDescriptor? AddProvider(object provider);
    IReadOnlySet<PluginDescriptor>? RemoveByFile(string filePath);
    IEnumerable<PluginDescriptor> Descriptors { get; }
    int Count { get; }
  }
}
