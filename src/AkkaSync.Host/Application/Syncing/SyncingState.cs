using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Pipelines;
using System.Collections.Immutable;

namespace AkkaSync.Host.Application.Syncing
{
  public sealed record SyncingState(ImmutableDictionary<string, IReadOnlyList<PluginInstance>> Instances): IStateSnashot
  {
    public static SyncingState Empty => new(ImmutableDictionary<string, IReadOnlyList<PluginInstance>>.Empty);
  }
}
