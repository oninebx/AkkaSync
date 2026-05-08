using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Plugins;

namespace AkkaSync.Core.Domain.Pipelines.Events
{
  public sealed record PipelineStarted(PipelineId Id, IReadOnlyDictionary<string, IPlugin> Plugins) : ISnapshotEvent
  {
    public IReadOnlyList<Type> SupportedTypes { get; init; } = [ typeof(PluginInstance), typeof(PipelineMetrics)];

    public IReadOnlyDictionary<Type, IReadOnlyList<string>> IdGroups { get; init; } = new Dictionary<Type, IReadOnlyList<string>>
    {
      [typeof(PluginInstance)] = [.. Plugins.Select(p => p.Key)],
      [typeof(PipelineMetrics)] = [ Id.Key ],
    };

    public IReadOnlySet<Type> ResetTypes { get; init; } = new HashSet<Type> { typeof(PluginInstance) };
  }
}
