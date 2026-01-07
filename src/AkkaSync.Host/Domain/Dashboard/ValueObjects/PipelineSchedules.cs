using System;

namespace AkkaSync.Host.Domain.Dashboard.ValueObjects;

public sealed record PipelineSchedules(
  IReadOnlyDictionary<string, string> Specs,
  IReadOnlyList<PipelineJob> Jobs) : IStoreValue
{
  public static PipelineSchedules Empty => new(
    Specs: new Dictionary<string, string>(),
    Jobs: []
  );
}
