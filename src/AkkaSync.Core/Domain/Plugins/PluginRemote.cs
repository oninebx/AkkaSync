using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Plugins
{
  public sealed record PluginRemote(string QualifiedName, string Version, string Provider) : ISnapshot
  {
    public string Identifier => Provider;
  }
}
