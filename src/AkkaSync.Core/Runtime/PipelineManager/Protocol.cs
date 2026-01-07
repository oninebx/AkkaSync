using System;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Core.Runtime.PipelineManager;

public static class PipelineManagerProtocol
{
  public sealed record Start();
  public sealed record StartPipeline(string Name);
}

