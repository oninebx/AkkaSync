using Akka.Actor;
using Akka.Event;
using Akka.Util;
using AkkaSync.Abstractions;
using AkkaSync.Core.PluginProviders;
using AkkaSync.Infrastructure.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure
{
  public class PluginManagerActor : ReceiveActor
  {
    private readonly IServiceProvider _serviceProvider;
    private readonly IPluginProviderRegistry<ISyncSource> _sourceRegistry;
    private readonly IPluginProviderRegistry<ISyncTransformer> _transformerRegistry;
    private readonly IPluginProviderRegistry<ISyncSink> _sinkRegistry;

    private readonly FileSystemWatcher _watcher;
    

    private readonly ILoggingAdapter _logger = Context.GetLogger();
    public PluginManagerActor(
      IServiceProvider serviceProvider,
      AkkaSyncOptions options,
      IPluginProviderRegistry<ISyncSource> sourceRegistry,
      IPluginProviderRegistry<ISyncTransformer> transformerRegistry,
      IPluginProviderRegistry<ISyncSink> sinkRegistry) 
    {
      _serviceProvider = serviceProvider;
      _sourceRegistry = sourceRegistry;
      _transformerRegistry = transformerRegistry;
      _sinkRegistry = sinkRegistry;
      _watcher = new FileSystemWatcher(options.PluginFolder, "AkkaSync.Plugins.*.dll")
      {
        NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
      };

      
    }

    protected override void PreStart()
    {
      _logger.Info("There are {0} source plugin(s), {1} transformer plugin(s) and {2} sink plugin(s) are being managed.", _sourceRegistry.Count, _transformerRegistry.Count, _sinkRegistry.Count);

      _watcher.Created += (s, e) => DoLoadPlugin(e.FullPath);

      _watcher.EnableRaisingEvents = true;
    }

    protected override void PostStop()
    {
      _watcher.Dispose();
      base.PostStop();
    }

    private void DoLoadPlugin(string path)
    {
      var plugins = PluginLoader.LoadFromFile(path, _serviceProvider);

      foreach (var plugin in plugins)
      {
        switch (plugin.InterfaceType)
        {
          case Type t when typeof(IPluginProvider<ISyncSource>).IsAssignableFrom(t):
            _sourceRegistry.AddProvider((IPluginProvider<ISyncSource>)plugin.ProviderInstance);
            break;
          case Type t when typeof(IPluginProvider<ISyncTransformer>).IsAssignableFrom(t):
            _transformerRegistry.AddProvider((IPluginProvider<ISyncTransformer>)plugin.ProviderInstance);
            break;
          case Type t when typeof(IPluginProvider<ISyncSink>).IsAssignableFrom(t):
            _sinkRegistry.AddProvider((IPluginProvider<ISyncSink>)plugin.ProviderInstance);
            break;
          //case Type t when typeof(IPluginProvider<IHistoryStore>).IsAssignableFrom(t):
          //  _historyRegistry.Add((IPluginProvider<IHistoryStore>)plugin.ProviderInstance);
          //  break;
        }
      }
      _logger.Info("Plugin {0} is loaded.", path);
    }

  }
}
