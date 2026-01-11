using System;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Core.Domain.Schedules;

public static class PipelineSchedulerProtocol
{
  public sealed record Start();
  public sealed record Trigger(string Name);
}
