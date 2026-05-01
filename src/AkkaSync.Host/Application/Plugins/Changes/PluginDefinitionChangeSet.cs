using AkkaSync.Abstractions;

namespace AkkaSync.Host.Application.Plugins.Changes
{
  public sealed record PluginDefinitionChangeSet(ChangeOperation Operation) : IChangeSet
  {
    public string Slice => "pluginConfig";
    public object Payload { get; set; } = default!;
  }
}
