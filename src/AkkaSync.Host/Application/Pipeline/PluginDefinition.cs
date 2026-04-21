namespace AkkaSync.Host.Application.Pipeline
{
  public sealed record PluginDefinition(string Key, string Name, string Type, string[] DependsOn);
}
