using System;
using AkkaSync.Host.Application.Store;

namespace AkkaSync.Host.Application.Scheduling;

public sealed record PipelineJob(string Name, DateTime NextUtc) : IStoreValue
{

}
