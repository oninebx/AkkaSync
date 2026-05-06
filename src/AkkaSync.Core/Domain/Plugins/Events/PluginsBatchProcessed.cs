namespace AkkaSync.Core.Domain.Plugins.Events
{
  public sealed record PluginsBatchProcessed(Dictionary<string, int> Processed, Dictionary<string, int> Errors);
}
