using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;
using AkkaSync.Host.Application.Pipeline;
using AkkaSync.Infrastructure.Messaging.Publish;
using Google.Protobuf.WellKnownTypes;

namespace AkkaSync.Host.Application.Dashboard.NotificationMappings
{
  public sealed class PipelineStateMapping : IEventNotificationMapping
  {
    public bool CanHandle(IStoreValue value) => value is PipelineState;

    public EventNotification? TryMap(IStoreValue store, INotificationEvent? @event)
    {
      var state = (PipelineState)store;

      return @event switch
      {
        SyncEngineReady e => new EventNotification("pipeline.specs.initialized", e.Pipelines),
        DashboardInitialized => new EventNotification("pipeline.specs.initialized", state.Pipelines),
        PipelineStartReported e => new EventNotification("pipeline.run.started", new { Id = e.PipelineId.Key, StartedAt = @event.OccurredAt }),
        PipelineCompleteReported e => new EventNotification("pipeline.run.completed", new { Id = e.PipelineId.Key, FinishAt = @event.OccurredAt }),
        _ => null
      };
    }
  }
}
