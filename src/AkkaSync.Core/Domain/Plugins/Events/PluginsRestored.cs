using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Plugins.Models;

namespace AkkaSync.Core.Domain.Plugins.Events
{
  
  public sealed record PluginsRestored(IReadOnlySet<PluginCacheEntry> Plugins) : ISnapshotEvent
  {
    public IReadOnlyList<Type> SupportedTypes => [typeof(PluginLocal)];

    public IReadOnlyDictionary<Type, IReadOnlyList<string>> IdGroups => new Dictionary<Type, IReadOnlyList<string>>
    {
      [typeof(PluginLocal)] = [.. Plugins.Select(p => p.Provider)]
    };
  }
}
