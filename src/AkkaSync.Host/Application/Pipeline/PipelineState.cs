using AkkaSync.Infrastructure.Messaging.Publish;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.Pipeline
{
  public sealed record PipelineState(ImmutableList<PipelineRecord> Pipelines): IStoreValue 
  {
    public static PipelineState Empty => new PipelineState([]);
  }
}
