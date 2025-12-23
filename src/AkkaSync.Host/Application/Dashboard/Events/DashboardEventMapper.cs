using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Events;
using AkkaSync.Host.Application.Dashboard.Events;

namespace AkkaSync.Host.Application.Dashboard.Events;

public static class DashboardEventMapper
{
  public static IDashboardEvent? TryMap(ISyncEvent evt) => evt switch
  {
    PipelineStarted e => new DashboardPipelineStarted(e.Id),
    _ => null
  };
}
