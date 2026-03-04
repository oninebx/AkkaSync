using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Infrastructure.SyncPlugins.PluginProviders;
using AkkaSync.Infrastructure.Messaging.Models;
using AkkaSync.Infrastructure.SyncPlugins.Loader;
using AkkaSync.Infrastructure.SyncPlugins.Storage;
using System.IO.Compression;
using Akka.DependencyInjection;
using AkkaSync.Infrastructure.Options;
using AkkaSync.Infrastructure.Messaging.Contract.Swap;

namespace AkkaSync.Infrastructure.Actors
{
  public class PluginManagerActor : ReceiveActor
  {
    private readonly IEnumerable<IPluginProviderRegistryAdapter> _registryAdapters;
    private readonly IServiceProvider _serviceProvider;
    private readonly IPluginStorage _pluginStorage;
    private readonly FileSystemWatcher _watcher;
    private readonly string _shadowFolder;
    private readonly Dictionary<string, PluginLoadContext> _pluginContexts;
    private readonly List<string> _pendingCleanup = [];
    private ICancelable? _cleanupSchedule;
    private IActorRef? _updateActor;

    private readonly ILoggingAdapter _logger = Context.GetLogger();
    public PluginManagerActor(
      IServiceProvider serviceProvider,
      IPluginStorage pluginStorage,
      AkkaSyncOptions options,
      IEnumerable<IPluginProviderRegistryAdapter> registryAdapters)
    {
      _serviceProvider = serviceProvider;
      _pluginStorage = pluginStorage;
      _registryAdapters = registryAdapters;
      _watcher = EnsureWatcherCreated(options.PluginFolder);
      _shadowFolder = options.ShadowFolder;
      _pluginContexts = options.PluginContexts;
      options.PluginContexts = null!;

      Receive<Protocol.CheckAndUpdatePlugins>(msg => DoCheckAndUpdate(msg.Required));
      ReceiveAsync<Protocol.LoadPlugin>(msg => DoLoadPlugin(msg.Path));
      Receive<Protocol.UnloadPlugin>(msg => DoUnloadPlugin(msg.Path));

      Receive<Protocol.CleanupPendingPlugins>(_ => DoCleanup());
    }

    protected override void PreStart()
    {
      
      var resolver = DependencyResolver.For(Context.System);
      _updateActor = Context.ActorOf(resolver.Props<PluginUpdateActor>(), "plugin-update");

      var stats = _registryAdapters.Select(adapter => (Name: adapter.GetType().GetGenericArguments().FirstOrDefault()?.Name ?? "Unknown", adapter.Count)).ToList();
      var message = string.Join(", ", stats.Select(s => $"{s.Count} {s.Name} plugin(s)"));
      _logger.Info("There are {0} being managed.", message);
      var plugins = _registryAdapters.SelectMany(adapter => adapter.Descriptors.Select(d => new PluginInfo(d.Name, d.Version)));
      if (plugins is not null && plugins.Any())
      {
        Context.System.EventStream.Publish(new PluginManagerInitialized(plugins.ToHashSet() ?? []));
      }

      var eventSelf = Self;
      _watcher.Created += (s, e) => eventSelf.Tell(new Protocol.LoadPlugin(e.FullPath));
      _watcher.Deleted += (s, e) => eventSelf.Tell(new Protocol.UnloadPlugin(e.FullPath));
      _watcher.EnableRaisingEvents = true;
    }

    protected override void PostStop()
    {
      _watcher.Dispose();
      base.PostStop();
    }

    private FileSystemWatcher EnsureWatcherCreated(string folder)
    {
      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }
      var watcher = new FileSystemWatcher(folder, "AkkaSync.Plugins.*.zip")
      {
        NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
      };
      _logger.Info("PluginManagerActor is watching folder {0} for plugin changes.", folder);
      return watcher;
    }

    private void DoCheckAndUpdate(IEnumerable<string> required)
    {
      _logger.Info("Received CheckAndUpdatePlugins message with required plugins: {0}", string.Join(", ", required));
      _pluginStorage.EnsureAsync(required);
    }

    private async Task DoLoadPlugin(string path)
    {
      await PluginLoader.EnsurePluginZipFileCreated(path);
      var shadowPath = ExtractToShadow(path);
      var (providers, context) = PluginLoader.LoadFromFile(shadowPath, _serviceProvider);
      foreach (var provider in providers)
      {
        var adapter = _registryAdapters.FirstOrDefault(r => r.CanHandle(provider.InterfaceType));
        if (adapter != null)
        {
          var descriptor = adapter.AddProvider(provider.ProviderInstance);
          if(descriptor is not null)
          {
            _logger.Info("PluginProvider {0} is added to registry.", provider.InterfaceType.Name);

            Context.System.EventStream.Publish(new PluginAdded(descriptor.Name, descriptor.Version));
          }
          
        }
      }
      if(context is not null)
      {
        _pluginContexts[Path.GetDirectoryName(shadowPath)!] = context;
      }
      
    }

    private bool DoUnloadPlugin(string path)
    {
      var shadowPath = Path.Combine(_shadowFolder, Path.GetFileNameWithoutExtension(path));
      foreach (var adapter in _registryAdapters)
      {
        var descriptors = adapter.RemoveByFile(shadowPath);
        if (descriptors != null && descriptors.Any())
        {
          _logger.Info("{0} PluginProvider(s) in file {1} is removed from registry.", descriptors.Count(), shadowPath);
          foreach (var descriptor in descriptors)
          {
            Context.System.EventStream.Publish(new PluginRemoved(descriptor.Name));
          }
        }
      }
      if (_pluginContexts.TryGetValue(shadowPath, out var context))
      {
        context.Unload();
        _pluginContexts.Remove(shadowPath);
        context = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        if (!TryDelete(shadowPath)) 
        { 
          _pendingCleanup.Add(shadowPath);
          _logger.Warning("Failed to unload Plugin Providers in {0}. Will retry later.", shadowPath);
          StartCleanupSchedule();
          return false;
        }
      }
      _logger.Info("Plugin Providers in {0} is unloaded.", shadowPath);
      return true;
    }

    private void DoCleanup() 
    {
      foreach(var path in _pendingCleanup.ToList())
      {
        if(TryDelete(path)) 
        { 
          _pendingCleanup.Remove(path); 
          _logger.Info("Pending cleanup for {0} is successful.", path); 
        }
      }

      if(_pendingCleanup.Count == 0) 
      { 
        _cleanupSchedule?.Cancel();
        _cleanupSchedule = null;
      }
    }

    private string ExtractToShadow(string path)
    {
      ZipFile.ExtractToDirectory(path, Path.Combine(_shadowFolder, Path.GetFileNameWithoutExtension(path)));
      var dllPath = GetShadowMainDllPath(Path.GetFileName(path));
      return dllPath;
    }

    private string GetShadowMainDllPath(string path) => Path.Combine(_shadowFolder, Path.GetFileNameWithoutExtension(path), Path.ChangeExtension(path, ".dll"));

    private void StartCleanupSchedule()
    {
      if (_cleanupSchedule != null)
      {
        return;
      }

      _cleanupSchedule = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
          TimeSpan.FromSeconds(1),
          TimeSpan.FromSeconds(2),
          Self,
          new Protocol.CleanupPendingPlugins(),
          Self
      );
    }

    private static bool TryDelete(string path)
    {
      try
      {
        if (Directory.Exists(path))
        {
          Directory.Delete(path, true);
        }
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
}
