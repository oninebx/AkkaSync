using System;
using AkkaSync.Host.Application.Dashboard;

namespace AkkaSync.Host.Application.Scheduling;

public sealed record PipelineJob(string Name, DateTime NextUtc) : IStoreValue;
