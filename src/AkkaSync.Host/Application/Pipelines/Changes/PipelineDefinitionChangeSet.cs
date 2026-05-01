using AkkaSync.Abstractions;

namespace AkkaSync.Host.Application.Pipelines.Changes
{
  public sealed record PipelineDefinitionChangeSet(ChangeOperation Operation) : IChangeSet
  {
    public string Slice => "pipelineConfig";

    public object Payload { get; set; } = default!;
  }
}
