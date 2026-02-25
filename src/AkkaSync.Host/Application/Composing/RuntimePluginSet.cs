using AkkaSync.Host.Application.Dashboard;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.Swapping
{
  public sealed record RuntimePluginSet(ImmutableHashSet<PluginEntry> Entries): IStoreValue
  {
    public static RuntimePluginSet EMPTY = new([]);
  }
}
