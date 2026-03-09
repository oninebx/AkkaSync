using System;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Host.Application.Scheduling;

public sealed record PipelineJob(string Id, DateTime NextUtc) : IStoreValue;
