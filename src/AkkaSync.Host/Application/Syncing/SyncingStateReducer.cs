using AkkaSync.Abstractions;
using AkkaSync.Core.Notifications;
using AkkaSync.Core.Projection;
using AkkaSync.Host.Application.Pipeline;

namespace AkkaSync.Host.Application.Syncing
{
  public static class SyncingStateReducer
  {
    public static SyncingState Reduce(SyncingState current, IProjectionEvent @event) => @event switch
    {
      PipelineCreatedTransition e => current with
      {
        Instances = current.Instances.SetItem(e.PipelineId.Key, [.. e.SourceInstances, .. e.TransformerInstances, ..e.SinkInstances])
      },
      _ => current
    };
  }
}
