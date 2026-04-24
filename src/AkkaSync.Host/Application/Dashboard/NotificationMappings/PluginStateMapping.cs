using AkkaSync.Abstractions;
using AkkaSync.Host.Application.Plugin;
using AkkaSync.Infrastructure.Messaging.Contract.Swap;
using AkkaSync.Infrastructure.Messaging.Contract.Update;
using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Host.Application.Dashboard.NotificationMappings
{
  public sealed class PluginStateMapping : IEventNotificationMapping
  {
    public bool CanHandle(IStateSnashot value) => value is PluginState;

    public EventNotification? TryMap(IStateSnashot store, IProjectionEvent? @event)
    {
      var state = (PluginState)store;
      return @event switch
      {
        PluginAdded e => new EventNotification("plugin.entries.added", state.Entries.FirstOrDefault(p => p.Id == e.Id)!),
        PluginRemoved e => new EventNotification("plugin.entries.removed", e.Name),
        PluginUpdated e => new EventNotification("plugin.entries.updated", new { e.Name, e.Version }),
        DashboardInitialized => new EventNotification("plugin.entries.initialized", state),
        PluginVersionsChecked e => new EventNotification("plugin.packages.checked", state.PackageEntries),
        PluginsRestored e => new EventNotification("plugin.entries.restored", state.Entries),
        _ => null
      };
    }
    }
}
