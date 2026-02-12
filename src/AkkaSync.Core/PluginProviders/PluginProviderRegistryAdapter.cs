using AkkaSync.Abstractions;

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

    public void AddProvider(object provider)
    {
      _registry.AddProvider((IPluginProvider<T>)provider);
    }

    public bool CanHandle(Type pluginType)
    {
      return typeof(IPluginProvider<T>).IsAssignableFrom(pluginType);
    }

    public bool RemoveByFile(string filePath)
    {
      if(!_registry.FileEntries.TryGetValue(filePath, out var keys) || keys is null)
      {
        return false;
      }
      foreach (var key in keys)
      {
        _registry.RemoveProvider(key);
      }
      return true;
    }
  }
}
