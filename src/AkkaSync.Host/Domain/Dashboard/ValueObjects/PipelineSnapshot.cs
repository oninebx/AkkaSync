using System;

namespace AkkaSync.Host.Domain.Dashboard.ValueObjects;

public record PipelineSnapshot(
  string Id,
  string Schedule)
{
  public DateTimeOffset? StartedAt {get; init;}
  public DateTimeOffset? FinishedAt {get; init;}
};
