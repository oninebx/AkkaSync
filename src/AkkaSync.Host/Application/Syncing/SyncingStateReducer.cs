using AkkaSync.Abstractions;
using AkkaSync.Core.Notifications;
using AkkaSync.Host.Application.Pipeline;

namespace AkkaSync.Host.Application.Syncing
{
  public static class SyncingStateReducer
  {
    public static SyncingState Reduce(SyncingState current, INotificationEvent @event) => @event switch
    {
      PipelineCreatedReported e => current with
      {
        Instances = current.Instances.SetItem(e.PipelineId.Key, [.. e.SourceInstances, .. e.TransformerInstances, e.SinkInstance])
      },
      _ => current
    };
  }
}
