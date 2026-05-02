using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.DataSources;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Plugins;

namespace AkkaSync.Core.Domain.Shared.Events
{
  public sealed record SyncEngineReady: ISnapshotEvent
  {
    public IReadOnlyList<PipelineSpec> Pipelines { get; init; }
    public IReadOnlyDictionary<string, ScheduleSpec> Schedules { get; init; }

    public IReadOnlyDictionary<string, (string Plugin, DataSourceMeta Connector)> DataSources { get; init; }
    public IReadOnlyDictionary<Type, IReadOnlyList<string>> IdGroups { get; init; }
    public IReadOnlyList<Type> SupportedTypes => [typeof(PipelineDefinition), typeof(PipelineMetrics), typeof(PluginDefinition), typeof(ConnectorDefinition)];

    public SyncEngineReady(IReadOnlyList<PipelineSpec> pipelines, IReadOnlyDictionary<string, ScheduleSpec> schedules)
    {
      Pipelines = pipelines;
      Schedules = schedules;

      var extractedDataSources = pipelines
          .SelectMany(p => p.Plugins)
          .Where(p => p is { Type: "source" or "sink", Meta.DataSource: not null })
          .Select(p => new { Plugin = p.Key, Connector = p.Meta!.DataSource! })
          .DistinctBy(ds => ds.Connector.Key)
          .ToDictionary(ds => ds.Connector.Key, ds => (ds.Plugin, ds.Connector));

      DataSources = extractedDataSources;

      IdGroups = new Dictionary<Type, IReadOnlyList<string>>
      {
        [typeof(PipelineDefinition)] = [.. pipelines.Select(p => p.Name)],
        [typeof(PipelineMetrics)] = [.. pipelines.Select(p => p.Name)],
        [typeof(PluginDefinition)] = [.. pipelines.SelectMany(p => p.Plugins).Select(p => p.Key)],
        [typeof(ConnectorDefinition)] = [.. extractedDataSources.Keys]
      };
    }
  }
}
