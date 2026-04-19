using AkkaSync.Abstractions.Models;
using System.Text.RegularExpressions;

namespace AkkaSync.Host.Application.Pipeline
{
  public sealed record PipelineRecord(string Id, DataSourceRecord Source, DataSourceRecord Target, IReadOnlyList<PluginRecord> Plugins)
  {
    public string? Name { get; init; }

    public DateTimeOffset? StartedAt { get; init; }
    public DateTimeOffset? FinishedAt { get; init; }

    public string? Schedule { get; init;  }
  }
}
