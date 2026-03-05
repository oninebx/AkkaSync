using AkkaSync.Abstractions;
using AkkaSync.Infrastructure.Messaging.Contract.Swap;
using AkkaSync.Infrastructure.Messaging.Contract.Update;

namespace AkkaSync.Host.Application.Swapping
{
  public static class PluginStateReducer
  {
    public static RuntimePluginSet Reduce(RuntimePluginSet current, INotificationEvent @event) => @event switch
    {
      PluginManagerInitialized e => current with { Entries = [..e.Plugins.Select(p => new PluginCacheInfo(p.Id, p.Version, PluginState.Loaded))] },
      PluginAdded e => current with { Entries = [ ..current.Entries.Select(entry => entry.Name == e.Name ? entry with { State = PluginState.Loaded } : entry)] },
      PluginRemoved e => current with { Entries = [..current.Entries.Select(entry => entry.Name == e.Name ? entry with { State = PluginState.Unloaded } : entry)] },
      PluginUpdated e => current with { Entries = [..current.Entries.Select(entry => entry.Name == e.Name ? entry with { Version = e.Version } : entry)] },
      PluginVersionsChecked e => current with { PackageEntries = [..e.NewVersions.Select(p => new PluginPackageInfo(p.Id, p.Version, p.Url))] },
      _ => current
    };
  }
}
