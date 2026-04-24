using AkkaSync.Abstractions.Models;

namespace AkkaSync.Core.Runtime;

public static class RegistryProtocol
{
  public sealed record Initialize(IDictionary<string, PipelineSpec> Pipelines);
  public sealed record CreatePipeline(string Key);
  public sealed record StartPipeline(string Key);
}

