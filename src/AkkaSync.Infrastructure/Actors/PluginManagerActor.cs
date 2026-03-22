using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Event;
using AkkaSync.Infrastructure.Messaging.Contract.Swap;
using AkkaSync.Infrastructure.SyncPlugins.Catalog;
using AkkaSync.Infrastructure.SyncPlugins.Loader;
using AkkaSync.Infrastructure.SyncPlugins.Storage;
using System.IO.Compression;
using static AkkaSync.Infrastructure.Messaging.Contract.Update.Protocol;
using static AkkaSync.Infrastructure.Messaging.Contract.Update.Request;

namespace AkkaSync.Infrastructure.Actors
{
  public class PluginManagerActor : ReceiveActor
  {
    private readonly IPluginStorage _pluginStorage;
    private readonly IPluginCatalog _pluginCatalog;
    private readonly FileSystemWatcher _watcher;
    private readonly string _shadowFolder;
    private readonly IActorRef _updateActor;
    private readonly IActorRef _loaderActor;

    private readonly ILoggingAdapter _logger = Context.GetLogger();
    public PluginManagerActor(
      IPluginCatalog pluginCatalog,
      IPluginStorage pluginStorage
      )
    {
      _pluginStorage = pluginStorage;
      _pluginCatalog = pluginCatalog;
      _watcher = EnsureWatcherCreated(pluginStorage.PluginFolder);
      _shadowFolder = pluginStorage.ShadowFolder;

      _updateActor = Context.ActorOf(DependencyResolver.For(Context.System).Props<PluginUpdaterActor>(), "plugin-updater");
      _loaderActor = Context.ActorOf(DependencyResolver.For(Context.System).Props<PluginLoaderActor>(), "plugin-loader");

      ReceiveAsync<Protocol.CleanupPendingPlugins>(_ => DoCleanup());

      Receive<CheckVersions>(_ => DoCheckVersions());
      Receive<UpdatePlugin>(msg => NotifyUpdatePlugin(msg));
    }

    protected override void PreStart()
    {

      Self.Tell(new Protocol.CleanupPendingPlugins());
      _loaderActor.Tell(new Protocol.RestorePlugins());

      _watcher.Created += async (s, e) => {
        await Task.Run(async () =>
        {
          await PluginLoader.EnsurePluginZipFileCreated(e.FullPath);
          var shadowPath = ExtractToShadow(e.FullPath);
          _loaderActor.Tell(new Protocol.LoadPlugin(shadowPath));
        });
       
      };
      _watcher.Deleted += (s, e) => _loaderActor.Tell(new Protocol.UnloadPlugin(ResolvePluginId(e.FullPath)));
      _watcher.EnableRaisingEvents = true;
    }

    protected override void PostStop()
    {
      _watcher.Dispose();
      base.PostStop();
    }

    private string ResolvePluginId(string fileName) => Path.GetFileNameWithoutExtension(fileName);

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

    private async Task DoCleanup() 
    {
      var pluginsToDelete = await _pluginCatalog.GetAllAsync(p => p.PendingDelete);
      try
      {
        await _pluginStorage.DeleteRangeAsync(pluginsToDelete.Select(p => p.Id));
        foreach(var plugin in pluginsToDelete)
        {
          await _pluginCatalog.RemoveAsync(plugin.Id, plugin.Version);
        }
      }
      catch (AggregateException ex) 
      {
        var errorMessages = ex.Flatten().InnerExceptions
        .Select(ex => ex.Message)
        .Distinct();

        _logger.Error("Delete plugins files failed: {0}",string.Join(" | ", errorMessages));
      }
     
    }

    private void DoCheckVersions()
    {
      _updateActor.Tell(new CheckVersionsForUpdate());
    }

    private void NotifyUpdatePlugin(UpdatePlugin msg)
    {
      _updateActor.Tell(new CheckoutNewVersion(msg.Url, msg.Checksum));
    }
    private string ExtractToShadow(string path)
    {
      ZipFile.ExtractToDirectory(path, Path.Combine(_shadowFolder, Path.GetFileNameWithoutExtension(path)));
      var dllPath = GetShadowMainDllPath(Path.GetFileName(path));
      return dllPath;
    }

    private string GetShadowMainDllPath(string path) => Path.Combine(_shadowFolder, Path.GetFileNameWithoutExtension(path), Path.ChangeExtension(path, ".dll"));
    
  }
}
