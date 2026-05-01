using AkkaSync.Abstractions;

namespace AkkaSync.Host.Application.Scheduling;

public sealed record PipelineJob(string Id, DateTime NextUtc) : ISnapshot
{
  public string Identifier => throw new NotImplementedException();
}