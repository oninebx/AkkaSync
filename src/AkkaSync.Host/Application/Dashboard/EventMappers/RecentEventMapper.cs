using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Events;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Application.Dashboard.EventMappers;

public static class RecentEventMapper
{
  public static RecentEvent? TryMap(ISyncEvent evt) => evt switch
  {
    PipelineStarted e => new RecentEvent(e.Id),
    _ => null
  };
}
