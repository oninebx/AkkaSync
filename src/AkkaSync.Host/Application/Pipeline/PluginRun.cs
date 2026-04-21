namespace AkkaSync.Host.Application.Pipeline
{
  public sealed record PluginRun(string Key, string Id, string Name, IReadOnlyList<string> DependsOn)
  {
    public int Processed { get; set; } = 0;
    public int Errors { get; set; } = 0;
  }
}
