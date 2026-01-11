using System;
using AkkaSync.Host.Application.Store;

namespace AkkaSync.Host.Domain.Dashboard.ValueObjects;

public sealed record PipelineJob(string Name, DateTime NextUtc) : IStoreValue
{

}
