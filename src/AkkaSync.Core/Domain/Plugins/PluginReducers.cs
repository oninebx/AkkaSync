using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Plugins.Events;
using AkkaSync.Core.Domain.Shared.Events;

namespace AkkaSync.Core.Domain.Plugins
{
  public static class PluginReducers
  {
    public static PluginDefinition ReduceDefinition(PluginDefinition? current, ISnapshotEvent @event, string id) => @event switch
    {
      SyncEngineReady ready => HandleSyncReadyForDefinition(current, id, ready),
      _ => throw new NotImplementedException()
    };

    public static PluginLocal ReduceLocal(PluginLocal? current, ISnapshotEvent @event, string id) => @event switch
    {
      PluginsRestored restored => HandleRestoredForLocal(current, id, restored),
      _ => throw new NotImplementedException()
    };

    public static PluginRemote ReduceRemote(PluginRemote? current, ISnapshotEvent @event, string id) => @event switch
    {
      PluginsVersionChecked versionChecked => HandleCheckedForRemote(current, id, versionChecked),
      _ => throw new NotImplementedException()
    };

    public static PluginInstance ReduceInstance(PluginInstance? current, ISnapshotEvent @event, string id) => @event switch
    {
      PipelineStarted started => HandleStartedForInstance(current, id, started),
      PipelineBatchProcessed processed => current! with { Processed = processed.MetricsData[id].ProcessedCount, Errors = processed.MetricsData[id].ErrorCount },
      WorkerStarted workerStarted => current! with { UsedBy = current.UsedBy + 1 },
      WorkerCompleted workerCompleted => current! with { UsedBy = current.UsedBy - 1 },
      _ => throw new NotImplementedException()
    };

    private static PluginDefinition HandleSyncReadyForDefinition(PluginDefinition? current, string id, SyncEngineReady e)
    {
      var spec = e.Pipelines
        .SelectMany(pipeline => pipeline.Plugins, (pipeline, plugin) => new { pipeline, plugin })
        .FirstOrDefault(x => x.plugin.Key == id) ?? throw new InvalidOperationException($"Spec not found for plugin: {id}");

      return current ?? new PluginDefinition(id, spec.plugin.Type, spec.plugin.Provider, spec.pipeline.Name) { DependsOn = spec.plugin.DependsOn };
    }

    private static PluginLocal HandleRestoredForLocal(PluginLocal? current, string id, PluginsRestored e)
    {
      var entry = e.Plugins.FirstOrDefault(p => p.Provider == id) ?? throw new InvalidOperationException($"Entry not found for plugin: {id}");
      
      return current ?? new PluginLocal(entry.QualifiedName, entry.Version, entry.Provider, entry.Kind);
    }

    private static PluginRemote HandleCheckedForRemote(PluginRemote? current, string id, PluginsVersionChecked e)
    {
      var entry = e.NewVersions.FirstOrDefault(e => e.Provider == id) ?? throw new InvalidOperationException($"Entry not found for plugin: { id }");
      return current ?? new PluginRemote(entry.QualifiedName, entry.Version, entry.Provider);
    }

    private static PluginInstance HandleStartedForInstance(PluginInstance? current, string id, PipelineStarted e)
    {
      var plugin = e.Plugins[id];
      return current ?? new PluginInstance(plugin.Id, plugin.Key);
    }
  }
}
