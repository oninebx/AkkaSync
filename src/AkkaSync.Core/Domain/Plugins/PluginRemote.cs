using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Plugins
{
  public sealed record PluginRemote(string QualifiedName, string Version, string Provider, string DownloadUrl, string CheckSum) : ISnapshot
  {
    public string Identifier => Provider;
  }
}
