using AkkaSync.Abstractions;

namespace AkkaSync.Host.Application.Plugins.Changes
{
  public sealed record PluginRemoteChangeSet(ChangeOperation Operation) : IChangeSet
  {
    public string Slice => "pluginCentral";

    public object Payload {get; set;} = default!;
  }
}
