using System;

namespace AkkaSync.Host.Domain.Dashboard.ValueObjects;

public sealed record PipelineJob(string Name, DateTime NextUtc) : IStoreValue
{

}
