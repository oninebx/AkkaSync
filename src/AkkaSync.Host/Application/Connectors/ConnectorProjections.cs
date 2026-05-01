using AkkaSync.Core.Common;
using AkkaSync.Core.Domain.DataSources;
using AkkaSync.Host.Application.DataSources.Changes;

namespace AkkaSync.Host.Application.DataSources
{
  public static class ConnectorProjections
  {
    public static IReadOnlyList<ConnectorDefinitionChangeSet> ProjectionDefinition(IReadOnlyList<ConnectorDefinition> currents, IReadOnlyList<ConnectorDefinition> nexts) =>
      [.. SnapshotDiffEngine.GenerateDiff<ConnectorDefinition, ConnectorDefinitionChangeSet>(currents, nexts)];
  }
}
