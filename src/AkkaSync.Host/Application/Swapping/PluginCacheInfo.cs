using System.Text.Json.Serialization;

namespace AkkaSync.Host.Application.Swapping
{
  public sealed record PluginCacheInfo(string Name, string Version, [property: JsonConverter(typeof(JsonStringEnumConverter))] PluginState State);

  public enum PluginState
  {
    Loaded = 1,
    Unloaded = 2,
  }
}
