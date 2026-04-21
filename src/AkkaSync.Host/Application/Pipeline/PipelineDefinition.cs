using AkkaSync.Abstractions.Models;
using System.Text.RegularExpressions;

namespace AkkaSync.Host.Application.Pipeline
{
  public sealed record PipelineDefinition(string Id, DataSourceRecord Source, DataSourceRecord Target, IReadOnlyList<PluginDefinition> Plugins)
  {
    public string? Name { get; init; }

    public string? Schedule { get; init;  }
  }
}
