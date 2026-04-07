using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;

namespace AkkaSync.Host.Application.Pipeline
{
  public static class PipelineReducer
  {
    public static PipelineState Reduce(PipelineState current, INotificationEvent @event) => @event switch
    {
      SyncEngineReady e => current with { Pipelines = [..e.Pipelines.Select(p => new PipelineRecord(p.Name, p.SourceProvider.Type, p.TransformerProvider.Type, p.SinkProvider.Type) 
      {
        Schedule = p.Schedule,
        Name = p.Name
      })] },
      PipelineStartReported e => current with
      {
        Pipelines = [..current.Pipelines.Select(p => p.Id == e.PipelineId.Key ?
        p with { StartedAt = @event.OccurredAt } : p)]
      },
      PipelineCompleteReported e => current with
      {
        Pipelines = [..current.Pipelines.Select(p => p.Id == e.PipelineId.Key ?
        p with { FinishedAt = @event.OccurredAt } : p)]
      },
      _ => current
    };
  }
}
