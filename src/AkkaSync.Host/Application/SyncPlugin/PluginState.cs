using AkkaSync.Abstractions;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.Plugin
{
  public sealed record PluginState (ImmutableHashSet<PluginCacheInfo> Entries, ImmutableHashSet<PluginPackageInfo> PackageEntries): IStateSnashot
  {
    public static PluginState EMPTY => new([],[]);
  }
}
