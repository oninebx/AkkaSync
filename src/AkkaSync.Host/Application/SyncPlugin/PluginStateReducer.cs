using AkkaSync.Abstractions;
using AkkaSync.Host.Application.Plugin;
using AkkaSync.Infrastructure.Messaging.Contract.Swap;
using AkkaSync.Infrastructure.Messaging.Contract.Update;

namespace AkkaSync.Host.Application.Swapping
{
  public static class PluginStateReducer
  {
    public static PluginState Reduce(PluginState current, INotificationEvent @event) => @event switch
    {
      PluginsRestored e => current with { Entries = [..e.Plugins.Select(p => new PluginCacheInfo(p.Id, p.Version, PluginStatus.Loaded))] },
      PluginAdded e => current with { Entries = [ ..current.Entries, new PluginCacheInfo(e.Id, e.Version, PluginStatus.Loaded)] },
      PluginRemoved e => current with { Entries = [..current.Entries.Select(entry => entry.Id == e.Name ? entry with { Status = PluginStatus.Unloaded } : entry)] },
      PluginUpdated e => current with { Entries = [..current.Entries.Select(entry => entry.Id == e.Name ? entry with { Version = e.Version } : entry)] },
      PluginVersionsChecked e => current with { PackageEntries = [..e.NewVersions.Select(p => new PluginPackageInfo(p.Id, p.Version, p.Url, p.Checksum))] },
      _ => current
    };
  }
}
