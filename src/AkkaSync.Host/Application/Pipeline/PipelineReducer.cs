using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;
using AkkaSync.Core.Projection;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.Pipeline
{
  public static class PipelineReducer
  {
    public static PipelineState Reduce(PipelineState current, IProjectionEvent @event) => @event switch
    {
      SyncEngineReady e => current with
      {
        Definitions = [..e.Pipelines.Select(p => new PipelineDefinition(
        p.Name,
        DataSourceRecord.From(p.Source.Meta?.DataSource),
        [.. p.Sinks.Select(sink => DataSourceRecord.From(sink.Meta?.DataSource))],
        [.. p.Plugins.Select(plugin => new PluginDefinition(plugin.Key, plugin.Provider, plugin.Type, plugin.DependsOn))]
      ) {
        Schedule = p.Schedule,
        Name = p.Name
      })]
      },
      PipelineCreatedTransition e => current with
      {
        Runs = current.Runs.Add(e.PipelineId, new PipelineRun(e.PipelineId,
        GeneratePlugins(e)))
      },
      WorkerMetricsReported e => ApplyMetrics(current, e),
      PipelineStartReported e => current with { Runs = current.Runs.SetItem(e.PipelineId, current.Runs[e.PipelineId] with { StartedAt = @event.OccurredAt }) },
      PipelineCompleteReported e => current with { Runs = current.Runs.SetItem(e.PipelineId, current.Runs[e.PipelineId] with { FinishedAt = @event.OccurredAt }) },


      //PipelineCreatedReported e => current with { Runs = [.. current.Runs, new PipelineRun(e.PipelineId, 
      //  e.SourceInstances.Concat(e.TransformerInstances).Append(e.SinkInstance).Select(x => new PluginRun(x.Key, x.Id, x.Name)).ToImmutableDictionary(p => p.Key, p => p))] },
      //PipelineStartReported e => current with{ Runs = [..current.Runs.Select(p => p.Id == e.PipelineId ? p with { StartedAt = @event.OccurredAt } : p)] },
      //PipelineCompleteReported e => current with { Runs = [..current.Runs.Select(p => p.Id == e.PipelineId ? p with { FinishedAt = @event.OccurredAt } : p)] },
      _ => current
    };

    private static PipelineState ApplyMetrics(PipelineState current, WorkerMetricsReported e)
    {
      var run = current.Runs.GetValueOrDefault(e.WorkerId.PipelineId);
      if (run is null)
        return current;

      var updatedPlugins = run.Plugins;

      foreach (var m in e.MetricsList)
      {
        updatedPlugins = updatedPlugins.SetItem(
            m.PluginId,
            updatedPlugins.TryGetValue(m.PluginId, out var plugin)
                ? plugin with
                {
                  Processed = plugin.Processed + m.ProcessedCount,
                  Errors = plugin.Errors + m.ErrorCount
                }
                : new PluginRun(m.PluginId, "", "", [])
                {
                  Processed = m.ProcessedCount,
                  Errors = m.ErrorCount
                }
        );
      }

      var updatedRun = run with
      {
        Plugins = updatedPlugins,
        Processed = run.Processed + e.MetricsList.Sum(x => x.ProcessedCount),
        Errors = run.Errors + e.MetricsList.Sum(x => x.ErrorCount)
      };

      return current with
      {
        Runs = current.Runs.SetItem(e.WorkerId.PipelineId, updatedRun)
      };
    }

    private static ImmutableDictionary<string, PluginRun> GeneratePlugins(PipelineCreatedTransition e)
      => e.SourceInstances.Concat(e.TransformerInstances).Concat(e.SinkInstances).Select(x => new PluginRun(x.Key, x.Id, x.Name, x.Dependencies)).ToImmutableDictionary(p => p.Id, p => p);
  }
}
