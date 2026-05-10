using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Plugins.Events
{
  public sealed record PluginAdded(string Name, string Holder, string Version, string Kind) : ISnapshotEvent
  {
    public IReadOnlyList<Type> SupportedTypes => [typeof(PluginLocal)];

    public IReadOnlyDictionary<Type, IReadOnlyList<string>> IdGroups => new Dictionary<Type, IReadOnlyList<string>>()
    {
      [typeof(PluginLocal)] = [Holder]
    };
  }
}
