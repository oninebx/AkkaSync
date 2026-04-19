namespace AkkaSync.Host.Application.Pipeline
{
  public sealed record PluginRecord(string Key, string Name, string Type, string[] DependsOn);
}
