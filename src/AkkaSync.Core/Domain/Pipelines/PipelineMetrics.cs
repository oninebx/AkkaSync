using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Pipelines
{
  public sealed record PipelineMetrics(string Name) : ISnapshot
  {
    public string Identifier => Name;
    public long TotalRuns { get; set; }
    public long TotalProcessed { get; set; }
    public long TotalErrors { get; set; }
    public DateTimeOffset? LastRun { get; set; }
    public DateTimeOffset? NextRun { get; set; }
    public PipelineStatus Status { get; set; }
    public string? InstanceId { get; set; }
  }
}
