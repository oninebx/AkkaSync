using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Events;
using AkkaSync.Host.Domain.Entities;

namespace AkkaSync.Host.Domain.States;

public static class HostStateReducer
{
  public static HostSnapshot Reduce(HostSnapshot current, ISyncEvent @event)
  {
    return @event switch
    {
      PipelineManagerStarted => current with { StartAt = @event.Timestamp},
      PipelineStarted e => current with { Pipelines =  current.Pipelines.Add(e.Id, PipelineSnapshot.FromId(e.Id))},
      _ => current
    };
  }
}
