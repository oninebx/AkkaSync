using AkkaSync.Infrastructure.Messaging.Publish;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.Plugin
{
  public sealed record PluginState (ImmutableHashSet<PluginCacheInfo> Entries, ImmutableHashSet<PluginPackageInfo> PackageEntries): IStoreValue
  {
    public static PluginState EMPTY => new([],[]);
  }
}
