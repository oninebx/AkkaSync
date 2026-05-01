using AkkaSync.Abstractions;

namespace AkkaSync.Host.Application.Pipelines.Changes
{
  public sealed record PipelineMetricsChangeSet(ChangeOperation Operation) : IChangeSet
  {
    public string Slice => "pipelineRuntime";

    public object Payload { get; set; } = default!;
  }
}
