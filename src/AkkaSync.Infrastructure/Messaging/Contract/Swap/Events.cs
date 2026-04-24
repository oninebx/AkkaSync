using AkkaSync.Abstractions;
using AkkaSync.Infrastructure.SyncPlugins.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.Messaging.Contract.Swap
{
  public sealed record PluginAdded(string Id, string Version): IProjectionEvent;
  public sealed record PluginRemoved(string Name): IProjectionEvent;
  public sealed record PluginUpdated(string Name, string Version): IProjectionEvent;
  //public sealed record PluginManagerInitialized(IReadOnlySet<PluginCacheEntry> Plugins): INotificationEvent;

  // PluginLoaderActor
  public sealed record PluginsRestored(IReadOnlySet<PluginCacheEntry> Plugins) : IProjectionEvent;

}
