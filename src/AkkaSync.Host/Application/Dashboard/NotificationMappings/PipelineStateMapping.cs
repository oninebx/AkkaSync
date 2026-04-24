using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;
using AkkaSync.Core.Projection;
using AkkaSync.Host.Application.Pipeline;
using AkkaSync.Infrastructure.Messaging.Publish;
using Google.Protobuf.WellKnownTypes;

namespace AkkaSync.Host.Application.Dashboard.NotificationMappings
{
  public sealed class PipelineStateMapping : IEventNotificationMapping
  {
    public bool CanHandle(IStateSnashot value) => value is PipelineState;

    public EventNotification? TryMap(IStateSnashot store, IProjectionEvent? @event)
    {
      var state = (PipelineState)store;

      return @event switch
      {
        SyncEngineReady e => new EventNotification("pipeline.specs.initialized", e.Pipelines),
        DashboardInitialized => new EventNotification("pipeline.specs.initialized", state.Definitions),
        PipelineCreatedTransition e => new EventNotification("pipeline.run.created", new { Id = e.PipelineId.Key, state.Runs[e.PipelineId].Plugins }),
        PipelineStartReported e => new EventNotification("pipeline.run.started", new { Id = e.PipelineId.Key, StartedAt = @event.OccurredAt }),
        PipelineCompleteReported e => new EventNotification("pipeline.run.completed", new { Id = e.PipelineId.Key, FinishAt = @event.OccurredAt }),
        WorkerMetricsReported e => new EventNotification("pipeline.metrics.updated", new { Id = e.WorkerId.PipelineId.Key, MetricsData = e.MetricsList }),
        _ => null
      };
    }
  }
}
