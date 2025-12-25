using System;

namespace AkkaSync.Host.Domain.Dashboard.ValueObjects;

public record PipelineSnapshot(string Id, int Progress, DateTimeOffset StartAt, DateTimeOffset? EndAt = null)
{
  public static PipelineSnapshot FromId(string Id)
  {
    return new PipelineSnapshot(Id, 0, DateTimeOffset.UtcNow);
  }
};
