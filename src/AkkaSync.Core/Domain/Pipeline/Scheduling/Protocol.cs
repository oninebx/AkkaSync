using System;
using AkkaSync.Core.Domain.Pipeline;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Core.Domain.Pipeline.Scheduling;

public static class PipelineSchedulerProtocol
{
  public sealed record Start();
  public sealed record Trigger(string Name);
}
