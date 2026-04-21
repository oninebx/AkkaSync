using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Infrastructure.Messaging.Publish;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.Pipeline
{
  public sealed record PipelineState(ImmutableList<PipelineDefinition> Definitions, ImmutableDictionary<PipelineId, PipelineRun> Runs): IStoreValue 
  {
    public static PipelineState Empty => new([], ImmutableDictionary<PipelineId, PipelineRun>.Empty);
  }
}
