using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Plugins.Models;

namespace AkkaSync.Core.Domain.Plugins.Events
{
  public sealed record PluginsVersionChecked(IReadOnlySet<PluginPackageEntry> NewVersions) : ISnapshotEvent
  {
    public IReadOnlyList<Type> SupportedTypes => [ typeof(PluginRemote) ];

    public IReadOnlyDictionary<Type, IReadOnlyList<string>> IdGroups => new Dictionary<Type, IReadOnlyList<string>>
    {
      [typeof(PluginRemote)] = [.. NewVersions.Select(p => p.QualifiedName)]
    };
  }
}
