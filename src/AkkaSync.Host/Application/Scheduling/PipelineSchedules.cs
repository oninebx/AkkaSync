using System;
using AkkaSync.Host.Application.Dashboard;

namespace AkkaSync.Host.Application.Scheduling;

public sealed record PipelineSchedules(
  IReadOnlyDictionary<string, string> Specs,
  IReadOnlyList<PipelineJob> Jobs) : IStoreValue
{
  public static PipelineSchedules Empty => new(
    Specs: new Dictionary<string, string>(),
    Jobs: []
  );
}
