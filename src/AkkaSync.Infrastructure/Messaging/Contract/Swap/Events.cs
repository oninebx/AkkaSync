using AkkaSync.Abstractions;
using AkkaSync.Infrastructure.SyncPlugins.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.Messaging.Contract.Swap
{
  public sealed record PluginAdded(string Id, string Version): INotificationEvent;
  public sealed record PluginRemoved(string Name): INotificationEvent;
  public sealed record PluginUpdated(string Name, string Version): INotificationEvent;
  public sealed record PluginManagerInitialized(IReadOnlySet<PluginCacheEntry> Plugins): INotificationEvent;

}
