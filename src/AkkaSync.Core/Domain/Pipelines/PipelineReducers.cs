using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Shared.Events;

namespace AkkaSync.Core.Domain.Pipelines
{
  public static class PipelineReducers
  {
    public static PipelineDefinition ReduceDefinition(PipelineDefinition? current, ISnapshotEvent @event, string id) => @event switch
    {
      SyncEngineReady ready => HandleSyncReadyForDefinition(current, id, ready),
      _ => throw new NotImplementedException(),
    };

    public static PipelineMetrics ReduceMetrics(PipelineMetrics? current, ISnapshotEvent @event, string id) => @event switch
    {
      SyncEngineReady ready => HandleSyncReadyForMetrics(current, id, ready),
      PipelineStarted started => (current ?? new PipelineMetrics(id)) with { Status = PipelineStatus.Running, InstanceId = started.Id.RunId.ToString() },
      PipelineCompleted completed => (current ?? new PipelineMetrics(id)) with { TotalRuns = current is null ? 0 : current.TotalRuns + 1, Status = PipelineStatus.Success },
      PipelineScheduled scheduled => (current ?? new PipelineMetrics(id)) with { NextRun = scheduled.NextUtc },
      PipelineSkipped skipped => (current ?? new PipelineMetrics(id)) with { Status = PipelineStatus.Skipped },
      _ => throw new NotFiniteNumberException()
    };

    private static PipelineDefinition HandleSyncReadyForDefinition(PipelineDefinition? current,string id, SyncEngineReady e)
    {
      var spec = e.Pipelines.FirstOrDefault(p => p.Name == id)
                 ?? throw new InvalidOperationException($"Spec not found for pipeline: {id}");
      var sourceKey = spec.Source?.Meta?.DataSource?.Key;
      var sinkKeys = spec.Sinks.Select(s => s.Meta?.DataSource?.Key).Where(key => key is not null).ToList();
      if (string.IsNullOrEmpty(sourceKey) || sinkKeys.Count == 0)
      {
        throw new InvalidOperationException($"Pipeline '{id}' is invalid: missing source or at least one valid sink connector.");
      }
      return (current ?? new PipelineDefinition(
        id, 
        spec.Plugins.ToDictionary(p => p.Key, p => new PluginInfo(p.Provider, p.Type)), 
        sourceKey, 
        [.. sinkKeys!]));
    }

    private static PipelineMetrics HandleSyncReadyForMetrics(PipelineMetrics? current, string id, SyncEngineReady e)
    {
      var spec = e.Pipelines.FirstOrDefault(p => p.Name == id)
                 ?? throw new InvalidOperationException($"Spec not found for pipeline: {id}");

      return (current ?? new PipelineMetrics(id));

    }

  }
}
