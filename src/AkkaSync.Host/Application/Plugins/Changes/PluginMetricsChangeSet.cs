using AkkaSync.Abstractions;

namespace AkkaSync.Host.Application.Plugins.Changes
{
  public sealed record PluginInstanceChangeSet(ChangeOperation Operation) : IChangeSet
  {
    public string Slice => "pluginRuntime";

    public object Payload { get; set; } = default!;
  }
}
