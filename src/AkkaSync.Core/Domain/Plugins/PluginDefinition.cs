using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.Plugins
{
  public sealed record PluginDefinition(string Key, string Type, string Provider, string Pipeline) : ISnapshot
  {
    public string Identifier => Key;
    public string[] DependsOn { get; set; } = [];
  }
}
