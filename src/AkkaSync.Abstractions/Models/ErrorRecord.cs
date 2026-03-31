
namespace AkkaSync.Abstractions.Models
{
  public sealed record ErrorRecord(string PId, string RunId, string PluginId, string Message)
  {
    public string? Context { get; init; }
    public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;
  }

  public sealed record ErrorQuery(
    string? PipelineId = null,
    string? PluginId = null,
    DateTime? From = null,
    DateTime? To = null
  );
}
