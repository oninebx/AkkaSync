namespace AkkaSync.Core.Domain.Plugins.Models
{
  public sealed record PluginCacheEntry(string QualifiedName, string Provider, string Version, string Kind);
}
