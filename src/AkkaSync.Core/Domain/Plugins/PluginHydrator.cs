using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Plugins.Commands;
using AkkaSync.Core.Domain.Plugins.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Core.Domain.Plugins
{
  public sealed class PluginHydrator : IContextHydrator
  {
    public bool CanHydrate(IRequestQuery query) => query switch
    {
      CheckoutPlugin => true,
      CheckForUpdates => true,
      _ => false
    };

    public IQueryPayload Hydrate(ISnapshotStore stateStore, IRequestQuery query)
    {
      return query switch {         
        CheckoutPlugin checkout => HydrateDownload(stateStore, checkout),
        CheckForUpdates updates => new CheckVersionsQuery(),
        _ => throw new InvalidOperationException($"Unsupported query type: {query.GetType().FullName}")
      };
    }

    private DownloadQuery HydrateDownload(ISnapshotStore store, CheckoutPlugin command)
    {
      var pluginId = command.Id;
      var pluginRemote = store.GetCurrentByType(typeof(PluginRemote))[pluginId];
      if (pluginRemote is PluginRemote remote)
      {
        var query = new DownloadQuery(pluginId, remote.DownloadUrl, remote.CheckSum);
        return query;
      }
      throw new InvalidOperationException($"Plugin with ID {pluginId} not found");

    }
  }
}
