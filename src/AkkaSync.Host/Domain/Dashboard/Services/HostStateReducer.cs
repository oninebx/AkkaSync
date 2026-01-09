using System;
using System.Collections.Immutable;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Pipeline;
using AkkaSync.Core.Runtime.PipelineManager;
using AkkaSync.Host.Domain.Dashboard;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Domain.Dashboard.Services;

public static class HostStateReducer
{
  public static HostSnapshot Reduce(HostSnapshot current, ISyncEvent @event)
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
