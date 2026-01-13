using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Runtime.PipelineManager;

namespace AkkaSync.Host.Application.Syncing;

public static class HostStateReducer
{
  public static SyncState Reduce(SyncState current, ISyncEvent @event)
  {
    return @event switch
    {
      PipelineManagerStarted e => current with { StartAt = @event.Timestamp, Pipelines = [..e.Pipelines.Select(p => new PipelineSnapshot(p.Name))] },
      PipelineStarted e => current with { Pipelines = [..current.Pipelines.Select(p => p.Id == e.PipelineId.Name ? 
        p with { StartedAt = e.Timestamp } : p)] },
      PipelineCompleted e => current with { Pipelines = [..current.Pipelines.Select(p => p.Id == e.PipelineId.Name ?
        p with { FinishedAt = e.Timestamp } : p)]},
      _ => current
    };
  }
}
