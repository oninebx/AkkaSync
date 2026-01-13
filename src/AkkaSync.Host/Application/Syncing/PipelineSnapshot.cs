using System;

namespace AkkaSync.Host.Application.Syncing;

public record PipelineSnapshot(string Id)
{
  public DateTimeOffset? StartedAt {get; init;}
  public DateTimeOffset? FinishedAt {get; init;}
};
