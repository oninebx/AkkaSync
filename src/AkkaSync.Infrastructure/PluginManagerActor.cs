using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Core.PluginProviders;
using AkkaSync.Infrastructure.DependencyInjection;

namespace AkkaSync.Infrastructure
{
  public class PluginManagerActor : ReceiveActor
  {
    private readonly IEnumerable<IPluginProviderRegistryAdapter> _registryAdapters;
    private readonly IServiceProvider _serviceProvider;
    private readonly IPluginStorage _pluginStorage;
    private readonly FileSystemWatcher _watcher;
    private readonly string _shadowFolder;
    private readonly Dictionary<string, ICancelable> _reloadTimers = [];
    private readonly Dictionary<string, PluginLoadContext> _pluginContexts;
    private readonly List<string> _pendingCleanup = [];
    private ICancelable? _cleanupSchedule;

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
      _watcher = new FileSystemWatcher(options.PluginFolder, "AkkaSync.Plugins.*.dll")
      {
        NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
      };
      _logger.Info("PluginManagerActor is watching folder {0} for plugin changes.", options.PluginFolder);

      _shadowFolder = options.ShadowFolder;
      _pluginContexts = options.PluginContexts;
      options.PluginContexts = null!;

      Receive<Protocol.CheckAndUpdatePlugins>(msg => DoCheckAndUpdate(msg.Required));
      Receive<Protocol.LoadPlugin>(msg => DoLoadPlugin(msg.Path));
      Receive<Protocol.UnloadPlugin>(msg => DoUnloadPlugin(msg.Path));
      Receive<Protocol.ReloadPlugin>(msg => 
      {
        if(_reloadTimers.TryGetValue(msg.Path, out var existing))
        {
          existing.Cancel();
        }
        var cancelable = Context.System.Scheduler.ScheduleTellOnceCancelable(
          TimeSpan.FromMilliseconds(500), Self, new Protocol.ReloadPluginInternal(msg.Path), Self);

        _reloadTimers[msg.Path] = cancelable;
      });

      Receive<Protocol.ReloadPluginInternal>(msg => DoReloadPlugin(msg.Path));
      Receive<Protocol.CleanupPendingPlugins>(_ => DoCleanup());
    }

    protected override void PreStart()
    {
      var stats = _registryAdapters.Select(adapter => (Name: adapter.GetType().GetGenericArguments().FirstOrDefault()?.Name ?? "Unknown", adapter.Count)).ToList();
      var message = string.Join(", ", stats.Select(s => $"{s.Count} {s.Name} plugin(s)"));
      _logger.Info("There are {0} being managed.", message);

      var eventSelf = Self;
      _watcher.Created += (s, e) => eventSelf.Tell(new Protocol.LoadPlugin(e.FullPath));
      _watcher.Deleted += (s, e) => eventSelf.Tell(new Protocol.UnloadPlugin(e.FullPath));
      _watcher.Changed += (s, e) => eventSelf.Tell(new Protocol.ReloadPlugin(e.FullPath));
      _watcher.EnableRaisingEvents = true;
    }

    protected override void PostStop()
    {
      _watcher.Dispose();
      base.PostStop();
    }

    private void DoCheckAndUpdate(IEnumerable<string> required)
    {
      _logger.Info("Received CheckAndUpdatePlugins message with required plugins: {0}", string.Join(", ", required));
      _pluginStorage.EnsureAsync(required);
    }

    private void DoLoadPlugin(string path)
    {
      var shadowPath = ShadowCopy(path);
      var (providers, context) = PluginLoader.LoadFromFile(shadowPath, _serviceProvider);
      foreach (var provider in providers)
      {
        var adapter = _registryAdapters.FirstOrDefault(r => r.CanHandle(provider.InterfaceType));
        if (adapter != null)
        {
          adapter.AddProvider(provider.ProviderInstance);
          _logger.Info("PluginProvider {0} is added to registry.", provider.InterfaceType.Name);
        }
      }
      _pluginContexts[shadowPath] = context!;
    }

    private bool DoUnloadPlugin(string path)
    {
      var shadowPath = GetShadowPath(path);
      foreach (var adapter in _registryAdapters)
      {
        if (adapter.RemoveByFile(shadowPath))
        {
          _logger.Info("PluginProvider(s) in file {0} is removed from registry.", shadowPath);
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

    private void DoReloadPlugin(string path)
    {
      var shadowPath = GetShadowPath(path);
      _reloadTimers.Remove(shadowPath);
      if (DoUnloadPlugin(shadowPath))
      {
        DoLoadPlugin(path);
      }
      
    }

    private void DoCleanup() 
    {
      foreach(var path in _pendingCleanup.ToList())
      {
        if(TryDelete(path)) 
        { 
          _pendingCleanup.Remove(path); 
          _logger.Info("Pending cleanup for {0} is successful.", path); 
          DoLoadPlugin(path);
        }
      }

      if(_pendingCleanup.Count == 0) 
      { 
        _cleanupSchedule?.Cancel();
        _cleanupSchedule = null;
      }
    }

    private string ShadowCopy(string path)
    {
      var shadowPath = GetShadowPath(path);
      File.Copy(path, shadowPath);
      return shadowPath;
    }

    private string GetShadowPath(string path) => Path.Combine(_shadowFolder, Path.GetFileName(path));

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

    private bool TryDelete(string path)
    {
      try
      {
        if (File.Exists(path))
        {
          File.Delete(path);
        }
        return true;
      }
      catch(Exception ex)
      {
        return false;
      }
    }
  }
}
