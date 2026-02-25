using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Core.PluginProviders
{
  public class PluginProviderRegistryAdapter<T> : IPluginProviderRegistryAdapter where T : class
  {
    private readonly IPluginProviderRegistry<T> _registry;
    public PluginProviderRegistryAdapter(IPluginProviderRegistry<T> registry) 
    {
      _registry = registry;
    }

    public int Count => _registry.Count;

    public IEnumerable<PluginDescriptor> Descriptors => _registry.ToDescriptors();

    public PluginDescriptor? AddProvider(object provider)
    {
      var typedProvider = (IPluginProvider<T>)provider;

      if (_registry.AddProvider(typedProvider))
      {
        return new PluginDescriptor(typedProvider.Key, typedProvider.Version.ToString());
      }
      return null;
    }

    public bool CanHandle(Type pluginType)
    {
      return typeof(IPluginProvider<T>).IsAssignableFrom(pluginType);
    }

    public IReadOnlySet<PluginDescriptor>? RemoveByFile(string filePath)
    {
      var result = new HashSet<PluginDescriptor>();
      var fileName = Path.GetFileNameWithoutExtension(filePath);
      if (_registry.FileEntries.TryGetValue(fileName, out var keys) && keys is not null)
      {
        foreach (var key in keys)
        {
          if (_registry.TryRemoveProvider(key, out var descriptor))
          {
            result.Add(descriptor);
          }
        }
      }
      return result;
    }
  }
}
