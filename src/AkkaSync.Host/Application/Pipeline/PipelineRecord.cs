namespace AkkaSync.Host.Application.Pipeline
{
  public sealed record PipelineRecord(string Id, string SourcePluginId, string TransformerPluginId, string SinkPluginId)
  {
    public string? Name { get; init; }
    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? FinishedAt { get; init; }
    public string? Schedule { get; init;  }
  }
}
