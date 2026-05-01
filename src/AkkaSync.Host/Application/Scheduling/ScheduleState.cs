using AkkaSync.Abstractions;

namespace AkkaSync.Host.Application.Scheduling;

public sealed record ScheduleState(
  IReadOnlyDictionary<string, string> Specs,
  IReadOnlyList<PipelineJob> Jobs) : ISnapshot
{
  public static ScheduleState Empty => new(
    Specs: new Dictionary<string, string>(),
    Jobs: []
  );

  public string Identifier => throw new NotImplementedException();
}
