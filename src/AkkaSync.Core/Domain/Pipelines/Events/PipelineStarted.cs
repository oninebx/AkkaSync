using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Plugins;

namespace AkkaSync.Core.Domain.Pipelines.Events
{
  public sealed record PipelineStarted(PipelineId Id, IReadOnlyDictionary<string, IPlugin> Plugins) : ISnapshotEvent
  {
    public IReadOnlyList<Type> SupportedTypes { get; init; } = [ typeof(PluginInstance)];

    public IReadOnlyDictionary<Type, IReadOnlyList<string>> IdGroups { get; init; } = new Dictionary<Type, IReadOnlyList<string>>
    {
      [typeof(PluginInstance)] = [.. Plugins.Select(p => p.Key)]
    };

  }
}
