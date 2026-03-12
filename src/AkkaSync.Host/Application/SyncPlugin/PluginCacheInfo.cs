using System.Text.Json.Serialization;

namespace AkkaSync.Host.Application.Plugin
{
  public sealed record PluginCacheInfo(string Id, string Version, [property: JsonConverter(typeof(JsonStringEnumConverter))] PluginStatus Status);

  public enum PluginStatus
  {
    Loaded = 1,
    Unloaded = 2,
  }
}
