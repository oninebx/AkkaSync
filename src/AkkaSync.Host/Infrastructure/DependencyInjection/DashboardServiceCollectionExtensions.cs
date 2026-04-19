using System;
using AkkaSync.Host.Application.Diagnosing;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Application.Query;
using AkkaSync.Host.Application.Query.Handlers;
using AkkaSync.Host.Application.Scheduling;
using AkkaSync.Host.Application.Swapping;
using AkkaSync.Host.Application.Syncing;
using AkkaSync.Host.Infrastructure.SignalR;
using AkkaSync.Host.Infrastructure.Stores;
using AkkaSync.Host.Application.Query.Mapper;
using AkkaSync.Infrastructure.Messaging.Publish;
using AkkaSync.Host.Application.Pipeline;
using AkkaSync.Host.Application.Dashboard.NotificationMappings;
using AkkaSync.Host.Application.Plugin;
using AkkaSync.Host.Application.SyncWorker;
using AkkaSync.Host.Application.RunningWorker;

namespace AkkaSync.Host.Infrastructure.Extensions;

public static class DashboardServiceExtension
{
  public static IServiceCollection AddDashboard(this IServiceCollection services)
  {
    services.AddSingleton<IEventNotificationMapper, DashboardEventMapper>();

    services.AddSingleton<IDashboardStore, InMemoryDashboardStore>();

    services.AddSingleton<IEventNotificationMapping, PipelineStateMapping>();
    services.AddSingleton<IEventNotificationMapping, ScheduleStateMapping>();
    services.AddSingleton<IEventNotificationMapping, PluginStateMapping>();
    services.AddSingleton<IEventNotificationMapping, WorkerStateMapping>();
    services.AddSingleton<IEventNotificationMapping, SyncingStateMapping>();

    services.AddSingleton(sp =>
      new EventReducerRegistryBuilder()
      .Add<PipelineState>(PipelineReducer.Reduce)
      .Add<SyncState>(SyncStateReducer.Reduce)
      .Add<ScheduleState>(ScheduleStateReducer.Reduce)
      .Add<DiagnosisJournal>(DiagnosisReducer.Reduce)
      .Add<PluginState>(PluginStateReducer.Reduce)
      .Add<WorkerState>(WorkerReducer.Reduce)
      .Add<SyncingState>(SyncingStateReducer.Reduce)
      .Build());

    services.AddSingleton<IDashboardClientRegistry, DashboardClientRegistry>();
    services.AddSingleton<IEventEnvelopePublisher, SignalREventEnvelopePublisher>();

    services.AddSingleton<IRequestQueryMapper, RequestQueryMapper>();
    services.AddSingleton<IDashboardQueryDispatcher, DashboardQueryDispatcher>();
    services.AddSingleton<IQueryHandler, QueryTestHandler>();
    return services;
  }
}
