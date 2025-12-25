using System;
using System.Collections.Immutable;
using AkkaSync.Abstractions;
using AkkaSync.Core.Events;
using AkkaSync.Host.Domain.Dashboard;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Domain.Dashboard.Services;

public static class HostStateReducer
{
  public static HostSnapshot Reduce(HostSnapshot current, ISyncEvent @event)
  {
    return @event switch
    {
      PipelineManagerStarted e => current with { StartAt = @event.Timestamp, PipelinesTotal = e.PipelinesTotal },
      PipelineStarted e => current with { Pipelines =  current.Pipelines.Add(PipelineSnapshot.FromId(e.Id))},
      PipelineCompleted e => current with { Pipelines = [.. current.Pipelines.Where(p => p.Id != e.Id)] },
      _ => current
    };
  }
}
