using System;

namespace AkkaSync.Host.Domain.Dashboard.ValueObjects;

public record PipelineSnapshot(
  string Id, 
  string Schedule)
{
  public static PipelineSnapshot FromId(string Id)
  {
    return new PipelineSnapshot(Id, "Every 10 mins");
  }
};
