using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.Pipeline
{
  public static class PipelineReducer
  {
    public static PipelineState Reduce(PipelineState current, INotificationEvent @event) => @event switch
    {
      SyncEngineReady e => current with { Definitions = [..e.Pipelines.Select(p => new PipelineDefinition(
        p.Name, 
        DataSourceRecord.From(p.Source.Meta?.DataSource), 
        DataSourceRecord.From(p.Sink.Meta?.DataSource),
        [.. p.Plugins.Select(plugin => new PluginDefinition(plugin.Key, plugin.Provider, plugin.Type, plugin.DependsOn))]
      ) {
        Schedule = p.Schedule,
        Name = p.Name
      })] },
      PipelineCreatedReported e => current with
      {
        Runs = current.Runs.Add(e.PipelineId, new PipelineRun(e.PipelineId,
        GeneratePlugins(e)))
      },

       PipelineStartReported e => current with{ Runs = current.Runs.SetItem(e.PipelineId, current.Runs[e.PipelineId] with { StartedAt = @event.OccurredAt }) },
       PipelineCompleteReported e => current with { Runs = current.Runs.SetItem(e.PipelineId, current.Runs[e.PipelineId] with { FinishedAt = @event.OccurredAt }) },
      //PipelineCreatedReported e => current with { Runs = [.. current.Runs, new PipelineRun(e.PipelineId, 
      //  e.SourceInstances.Concat(e.TransformerInstances).Append(e.SinkInstance).Select(x => new PluginRun(x.Key, x.Id, x.Name)).ToImmutableDictionary(p => p.Key, p => p))] },
      //PipelineStartReported e => current with{ Runs = [..current.Runs.Select(p => p.Id == e.PipelineId ? p with { StartedAt = @event.OccurredAt } : p)] },
      //PipelineCompleteReported e => current with { Runs = [..current.Runs.Select(p => p.Id == e.PipelineId ? p with { FinishedAt = @event.OccurredAt } : p)] },
      _ => current
    };

    private static ImmutableDictionary<string, PluginRun> GeneratePlugins(PipelineCreatedReported e) 
      => e.SourceInstances.Concat(e.TransformerInstances).Append(e.SinkInstance).Select(x => new PluginRun(x.Key, x.Id, x.Name, x.Dependencies)).ToImmutableDictionary(p => p.Id, p => p);
  }
}
