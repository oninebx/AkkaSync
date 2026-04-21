using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Core.Domain.Shared;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.Pipeline
{
  public sealed record PipelineRun(PipelineId Id, ImmutableDictionary<string, PluginRun> Plugins)
  {
    public int Processed { get; set; } = 0;
    public int Errors { get; set; } = 0;
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset FinishedAt { get; set; }
  }
}
