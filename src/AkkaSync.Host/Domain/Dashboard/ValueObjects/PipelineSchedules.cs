using System;

namespace AkkaSync.Host.Domain.Dashboard.ValueObjects;

public sealed record PipelineSchedules(IReadOnlyDictionary<string, IReadOnlyList<string>> Data) : IStoreValue
{
  public static PipelineSchedules Empty => new(
    Data: new Dictionary<string, IReadOnlyList<string>>()
  );
}
