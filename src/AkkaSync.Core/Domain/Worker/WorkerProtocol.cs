using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Pipeline;

namespace AkkaSync.Core.Domain.Worker;

public static class WorkerProtocol
{
  public sealed record Create(PipelineId PipelineId, ISyncSource Source);
  public sealed record Start();
}
