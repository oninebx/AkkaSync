using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Plugins;
using AkkaSync.Core.Domain.Workers;

namespace AkkaSync.Core.Domain.Pipelines.Events
{
  public sealed record PipelineBatchProcessed(IReadOnlyDictionary<string, PluginMetrics> MetricsData): ISnapshotEvent
  {
    public IReadOnlyList<Type> SupportedTypes => [typeof(PluginInstance)];

    public IReadOnlyDictionary<Type, IReadOnlyList<string>> IdGroups => new Dictionary<Type, IReadOnlyList<string>>
    {
      [typeof(PluginInstance)] = [.. MetricsData.Keys]
    };
  }
}
