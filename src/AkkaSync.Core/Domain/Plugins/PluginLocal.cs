using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Plugins
{
  public sealed record PluginLocal(string Name, string Version, string Provider, string Kind) : ISnapshot
  {
    public string Identifier => Provider;
  }
}
