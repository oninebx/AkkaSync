using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Pipelines;

namespace AkkaSync.Core.Domain.Workers;

public static class WorkerProtocol
{
  public sealed record Create(PipelineId PipelineId, ISyncSource Source, string? Cursor = null);
}
