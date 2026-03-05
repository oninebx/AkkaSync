using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Infrastructure.Messaging.Publish;
using AkkaSync.Infrastructure.SyncPlugins.Models;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.Swapping
{
  public sealed record RuntimePluginSet(ImmutableHashSet<PluginCacheInfo> Entries, ImmutableHashSet<PluginPackageInfo> PackageEntries): IStoreValue
  {
    public static RuntimePluginSet EMPTY => new([],[]);
  }
}
