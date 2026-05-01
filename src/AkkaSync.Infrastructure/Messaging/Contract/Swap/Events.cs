using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Plugins;
using AkkaSync.Infrastructure.SyncPlugins.Models;

namespace AkkaSync.Infrastructure.Messaging.Contract.Swap
{
  public sealed record PluginAdded(string Id, string Version): IProjectionEvent;
  public sealed record PluginRemoved(string Name): IProjectionEvent;
  public sealed record PluginUpdated(string Name, string Version): IProjectionEvent;
  //public sealed record PluginManagerInitialized(IReadOnlySet<PluginCacheEntry> Plugins): INotificationEvent;

}
