using System;
using AkkaSync.Core.Application.Diagnosis;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Application.Diagnosis;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Application.Query;
using AkkaSync.Host.Application.Query.Handlers;
using AkkaSync.Host.Domain.Dashboard.Services;
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

    services.AddSingleton<IDashboardStore, InMemoryDashboardStore>();
    services.AddSingleton(sp => 
      new EventReducerRegistryBuilder()
      .Add<HostSnapshot>(HostStateReducer.Reduce)
      .Add<PipelineSchedules>(ScheduleStateReducer.Reduce)
      .Add<DiagnosisJournal>(DiagnosisReducer.Reduce)
      .Build());

    services.AddSingleton<IDashboardClientRegistry, DashboardClientRegistry>();
    services.AddSingleton<IEventEnvelopePublisher, SignalREventEnvelopePublisher>();

    services.AddSingleton<IDashboardQueryDispatcher, DashboardQueryDispatcher>();
    services.AddSingleton<IQueryHandler, QueryTestHandler>();
    return services;
  }
}
