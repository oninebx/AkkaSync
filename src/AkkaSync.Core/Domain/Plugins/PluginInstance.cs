using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Plugins
{
  public sealed record PluginInstance(string Id, string Key) : ISnapshot
  {
    public int Processed { get; set; } = 0;
    public int Errors { get; set; } = 0;
    public string Identifier => Id;
  }
}
