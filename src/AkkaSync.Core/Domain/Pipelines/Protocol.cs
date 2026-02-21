using System;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Core.Domain.Pipelines;

public static class PipelineProtocol
{
  public sealed record Create(RunId RunId, string Name);
}
