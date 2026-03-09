using System;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Host.Application.Scheduling;

public sealed record ScheduleState(
  IReadOnlyDictionary<string, string> Specs,
  IReadOnlyList<PipelineJob> Jobs) : IStoreValue
{
  public static ScheduleState Empty => new(
    Specs: new Dictionary<string, string>(),
    Jobs: []
  );
}
