using AkkaSync.Infrastructure.SyncPlugins.PluginProviders;

namespace AkkaSync.Infrastructure.Options
{
  public sealed record AkkaSyncOptions
  {
    public string PluginFolder { get; set; } = string.Empty;
    public string ShadowFolder { get; set; } = string.Empty;
    public Dictionary<string, PluginLoadContext> PluginContexts { get; set; } = [];
  }
}
