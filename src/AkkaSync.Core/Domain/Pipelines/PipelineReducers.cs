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
      PipelineCompleted completed => (current ?? new PipelineMetrics(id)) with { TotalRuns = current is null ? 0 : current.TotalRuns + 1 },
      _ => throw new NotFiniteNumberException()
    };

    private static PipelineDefinition HandleSyncReadyForDefinition(PipelineDefinition? current,string id, SyncEngineReady e)
    {
      var spec = e.Pipelines.FirstOrDefault(p => p.Name == id)
                 ?? throw new InvalidOperationException($"Spec not found for pipeline: {id}");

      return (current ?? new PipelineDefinition(id, spec.Plugins.ToDictionary(p => p.Key, p => new PluginInfo(p.Provider, p.Type)))) with
      {
        Schedule = spec.Schedule
      };
    }

    private static PipelineMetrics HandleSyncReadyForMetrics(PipelineMetrics? current, string id, SyncEngineReady e)
    {
      var spec = e.Pipelines.FirstOrDefault(p => p.Name == id)
                 ?? throw new InvalidOperationException($"Spec not found for pipeline: {id}");

      return (current ?? new PipelineMetrics(id));
    }
  }
}
