using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Pipelines.Events
{
  public sealed record PipelineCompleted(PipelineId PipelineId) : ISnapshotEvent
  {
    public IReadOnlyList<Type> SupportedTypes => [typeof(PipelineMetrics)];

    public IReadOnlyDictionary<Type, IReadOnlyList<string>> IdGroups => new Dictionary<Type, IReadOnlyList<string>>
    {
      [typeof(PipelineMetrics)] = [ PipelineId.Key ]
    };
  }
}
