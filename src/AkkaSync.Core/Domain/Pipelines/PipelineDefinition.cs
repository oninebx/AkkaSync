using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Pipelines
{
  public sealed record PluginInfo(string Provider, string Kind);
  public sealed record PipelineDefinition(string Name, Dictionary<string, PluginInfo> Plugins, string SourceId, string[] SinkIds) : ISnapshot
  {
    public string Identifier => Name;
  }
}
