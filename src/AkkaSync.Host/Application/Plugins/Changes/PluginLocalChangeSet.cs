using AkkaSync.Abstractions;

namespace AkkaSync.Host.Application.Plugins.Changes
{
  public sealed record PluginLocalChangeSet(ChangeOperation Operation) : IChangeSet
  {
    public string Slice => "pluginCache";


    public object Payload { get; set; } = default!;
  }
}
