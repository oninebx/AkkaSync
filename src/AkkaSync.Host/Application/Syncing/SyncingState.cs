using AkkaSync.Core.Domain.Pipelines;
using AkkaSync.Infrastructure.Messaging.Publish;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.Syncing
{
  public sealed record SyncingState(ImmutableDictionary<string, IReadOnlyList<PluginInstance>> Instances): IStoreValue
  {
    public static SyncingState Empty => new(ImmutableDictionary<string, IReadOnlyList<PluginInstance>>.Empty);
  }
}
