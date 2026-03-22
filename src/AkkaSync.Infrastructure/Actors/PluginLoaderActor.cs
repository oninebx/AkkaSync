using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Infrastructure.Messaging.Contract.Swap;
using AkkaSync.Infrastructure.SyncPlugins.Catalog;
using AkkaSync.Infrastructure.SyncPlugins.Loader;
using AkkaSync.Infrastructure.SyncPlugins.Models;
using AkkaSync.Infrastructure.SyncPlugins.PluginProviders;
using AkkaSync.Infrastructure.SyncPlugins.Storage;

namespace AkkaSync.Infrastructure.Actors
{
  public sealed class PluginLoaderActor: ReceiveActor
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<IPluginProviderRegistryAdapter> _registryAdapters;
    private readonly ILoggingAdapter _logger = Context.GetLogger();
    private readonly Dictionary<string, PluginLoadContext> _pluginContexts;
    private readonly IPluginCatalog _pluginCatalog;
    private readonly IPluginStorage _pluginStorage;

    public PluginLoaderActor(
      IServiceProvider serviceProvider,
      IPluginCatalog catalog, 
      IPluginStorage storage, 
      IEnumerable<IPluginProviderRegistryAdapter> registryAdapters) 
    {
      _serviceProvider = serviceProvider;
      _registryAdapters = registryAdapters;
      _pluginCatalog = catalog;
      _pluginStorage = storage;
      _pluginContexts = [];

      ReceiveAsync<Protocol.RestorePlugins>(_ => HandleRestore());
      ReceiveAsync<Protocol.LoadPlugin>(msg => DoLoadPlugin(msg.PluginId));
      ReceiveAsync<Protocol.UnloadPlugin>(msg => DoUnloadPlugin(msg.PluginId));
    }

    private async Task HandleRestore()
    {
      var registeredPlugins = await _pluginCatalog.GetAllAsync(p => !p.PendingDelete);

      var registeredPluginFileNames = registeredPlugins.Select(p => p.Id).ToHashSet();
      var pluginFiles = _pluginStorage.GetPluginFiles().Where(f => registeredPluginFileNames.Contains(Path.GetFileNameWithoutExtension(f)));
      foreach (var file in pluginFiles)
      {
        RegisterPlugin(file);
      }
      var stats = _registryAdapters.Select(adapter => (Name: adapter.GetType().GetGenericArguments().FirstOrDefault()?.Name ?? "Unknown", adapter.Count)).ToList();
      var message = string.Join(", ", stats.Select(s => $"{s.Count} {s.Name} plugin(s)"));
      _logger.Info("There are {0} being managed.", message);

      var loadedPlugins = _registryAdapters.SelectMany(adapter => adapter.Descriptors.Select(d => new PluginCacheEntry(d.Holder, d.Version))).ToHashSet();

      var validLoadedPlugins = new HashSet<PluginCacheEntry>();
      foreach (var plugin in registeredPlugins)
      {
        var loadedPlugin = loadedPlugins.FirstOrDefault(p => p.Id == plugin.Id);
        if (loadedPlugin == null)
        {
          await _pluginCatalog.UpdateAsync(plugin with { PendingDelete = true });
        }
        else
        {
          validLoadedPlugins.Add(loadedPlugin);
        }
      }

      if (validLoadedPlugins.Count != 0)
      {
        Context.System.EventStream.Publish(new PluginsRestored(validLoadedPlugins));
      }
    }

    private async Task DoLoadPlugin(string path)
    {
      var descriptor = RegisterPlugin(path);

      if (descriptor is not null)
      {
        Context.System.EventStream.Publish(new PluginAdded(descriptor.Holder, descriptor.Version));
      }
    }

    private async Task<bool> DoUnloadPlugin(string path)
    {
      string version = string.Empty;
      foreach (var adapter in _registryAdapters)
      {
        var descriptors = adapter.RemoveByFile(path);
        if (descriptors != null && descriptors.Any())
        {
          _logger.Info("{0} PluginProvider(s) in file {1} is removed from registry.", descriptors.Count(), path);
          foreach (var descriptor in descriptors)
          {
            Context.System.EventStream.Publish(new PluginRemoved(descriptor.Name));
            if (string.IsNullOrEmpty(version))
            {
              version = descriptor.Version;
            }
          }
        }
      }
      var fileKey = Path.GetFileName(path);

      var entries = await _pluginCatalog.GetAllAsync(p => p.Id == fileKey && p.Version == version);
      if (entries != null && entries.Count == 1)
      {
        var entryToUpdate = entries[0];
        await _pluginCatalog.UpdateAsync(entryToUpdate with { PendingDelete = true});
      }

      if (_pluginContexts.TryGetValue(fileKey, out var context))
      {

        context.Unload();
        _pluginContexts.Remove(fileKey);
        context = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
      }
      return true;
    }

    private PluginDescriptor? RegisterPlugin(string file)
    {
      var (providers, context) = PluginLoader.LoadFromFile(file, _serviceProvider);
      PluginDescriptor? descriptor = null;
      foreach (var provider in providers)
      {
        var adapter = _registryAdapters.FirstOrDefault(r => r.CanHandle(provider.InterfaceType));
        if (adapter != null)
        {
          descriptor = adapter.AddProvider(provider.ProviderInstance);
          if (descriptor is not null)
          {
            _logger.Info("PluginProvider {0} is added to registry.", provider.InterfaceType.Name);
          }
        }
      }
      if (context is not null)
      {
        _pluginContexts[Path.GetFileNameWithoutExtension(file)!] = context;
      }
      return descriptor;
    }
  }
}
