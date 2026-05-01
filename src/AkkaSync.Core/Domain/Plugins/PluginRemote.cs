using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Plugins
{
  public sealed record PluginRemote(string QualifiedName, string Version) : ISnapshot
  {
    public string Identifier => QualifiedName;
  }
}
