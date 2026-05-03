using AkkaSync.Core.Common;
using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Host.Application.Pipelines.Changes;

namespace AkkaSync.Host.Application.Pipelines
{
  public static class PipelineProjections
  {
    public static IReadOnlyList<PipelineDefinitionChangeSet> ProjectionDefinition(IReadOnlyList<PipelineDefinition> currents, IReadOnlyList<PipelineDefinition> nexts)
      => [.. SnapshotDiffEngine.GenerateDiff<PipelineDefinition, PipelineDefinitionChangeSet>(currents, nexts)];

    public static IReadOnlyList<PipelineMetricsChangeSet> ProjectionMetrics(IReadOnlyList<PipelineMetrics> currents, IReadOnlyList<PipelineMetrics> nexts)
      => [.. SnapshotDiffEngine.GenerateDiff<PipelineMetrics, PipelineMetricsChangeSet>(currents, nexts)];
  }
}
