using System;
using AkkaSync.Host.Application.Common;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Application.Query;
using AkkaSync.Host.Application.Query.Handlers;
using AkkaSync.Host.Domain.Dashboard.Repositories;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;
using AkkaSync.Host.Infrastructure.SignalR;
using AkkaSync.Host.Infrastructure.Stores;

namespace AkkaSync.Host.Infrastructure.Extensions;

public static class DashboardServiceExtension
{
  public static IServiceCollection AddDashboard(this IServiceCollection services)
  {
    services.AddSingleton<IEventIdGenerator, GuidEventIdGenerator>();
    services.AddSingleton<ISequenceGenerator, InMemorySequenceGenerator>();
    services.AddSingleton<IEventEnvelopeFactory, EventEnvelopeFactory>();

    services.AddSingleton<IReplayStore<HostSnapshot>, InMemoryHostStateStore>();
    services.AddSingleton<IHostStateStore, InMemoryHostStateStore>();
    services.AddSingleton<IDashboardClientRegistry, DashboardClientRegistry>();
    services.AddSingleton<IEventEnvelopePublisher, SignalREventEnvelopePublisher>();

    services.AddSingleton<IDashboardQueryDispatcher, DashboardQueryDispatcher>();
    services.AddSingleton<IQueryHandler, QueryTestHandler>();

    // services.AddSingleton<IHostSnapshotPublisher, SignalRHostSnapshotPublisher>();
    // builder.Services.AddSingleton<IEventEnvelopePublisher, SignalREventEnvelopePublisher>();
    
    // services.AddSingleton<IDashboardEventStore, InMemoryDashboardEventStore>();
    return services;
  }
}
