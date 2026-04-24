namespace AkkaSync.Core.Domain.Schedules;

public static class SchedulerProtocol
{
  public sealed record Initialize(IReadOnlyDictionary<string, string> Schedules);
  public sealed record Trigger(string Key);
}
