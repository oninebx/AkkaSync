using AkkaSync.Core.Common;
using AkkaSync.Core.Domain.Plugins;
using AkkaSync.Host.Application.Plugins.Changes;

namespace AkkaSync.Host.Application.Plugins
{
  public static class PluginProjections
  {
    public static IReadOnlyList<PluginDefinitionChangeSet> ProjectionDefinition(IReadOnlyList<PluginDefinition> currents, IReadOnlyList<PluginDefinition> nexts) =>
      [.. SnapshotDiffEngine.GenerateDiff<PluginDefinition, PluginDefinitionChangeSet>(currents, nexts)];

    public static IReadOnlyList<PluginLocalChangeSet> ProjectionLocal(IReadOnlyList<PluginLocal> currents, IReadOnlyList<PluginLocal> nexts) =>
      [.. SnapshotDiffEngine.GenerateDiff<PluginLocal, PluginLocalChangeSet>(currents, nexts)];

    public static IReadOnlyList<PluginRemoteChangeSet> ProjectionRemote(IReadOnlyList<PluginRemote> currents, IReadOnlyList<PluginRemote> nexts) =>
      [.. SnapshotDiffEngine.GenerateDiff<PluginRemote, PluginRemoteChangeSet>(currents, nexts)];
    public static IReadOnlyList<PluginInstanceChangeSet> ProjectionInstance(IReadOnlyList<PluginInstance> currents, IReadOnlyList<PluginInstance> nexts) =>
      [.. SnapshotDiffEngine.GenerateDiff<PluginInstance, PluginInstanceChangeSet>(currents, nexts)];
  }
}
