using System;

namespace AkkaSync.Host.Application.Dashboard.Events;

public record DashboardPipelineStarted(string Id) : IDashboardEvent
{
  public DateTimeOffset Timestamp => DateTimeOffset.UtcNow;
}