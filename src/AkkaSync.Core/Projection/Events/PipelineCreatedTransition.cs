using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Pipelines;

namespace AkkaSync.Core.Projection.Events
{
  public sealed record PipelineCreatedTransition(PipelineId PipelineId): IProjectionEvent
  {
    public IReadOnlyList<PluginInstance> SourceInstances { get; set; } = [];
    public IReadOnlyList<PluginInstance> TransformerInstances { get; set; } = [];
    public IReadOnlyList<PluginInstance> SinkInstances { get; set; } = [];
  }
}
