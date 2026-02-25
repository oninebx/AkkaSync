using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;

namespace AkkaSync.Host.Application.Syncing;

public static class SyncStateReducer
{
  public static SyncState Reduce(SyncState current, INotificationEvent @event)
  {
    return @event switch
    {
      SyncEngineReady e => current with { StartAt = @event.OccurredAt, Pipelines = [..e.Pipelines.Select(p => new PipelineSnapshot(p.Name))] },
      PipelineStartReported e => current with { Pipelines = [..current.Pipelines.Select(p => p.Id == e.PipelineId.Name ? 
        p with { StartedAt = @event.OccurredAt } : p)] },
      PipelineCompleteReported e => current with { Pipelines = [..current.Pipelines.Select(p => p.Id == e.PipelineId.Name ?
        p with { FinishedAt = @event.OccurredAt } : p)]},
      _ => current
    };
  }
}
