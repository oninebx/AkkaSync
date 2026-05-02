using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Shared.Events;

namespace AkkaSync.Core.Domain.DataSources
{
  public static class ConnectorReducers
  {
    public static ConnectorDefinition ReduceDefinition(ConnectorDefinition? current, ISnapshotEvent @event, string id) => @event switch
    {
      SyncEngineReady ready => HandleReadyForDefinition(current, id, ready),
      _ => throw new NotImplementedException()
    };

    private static ConnectorDefinition HandleReadyForDefinition(ConnectorDefinition? current, string id, SyncEngineReady ready)
    {
      var (plugin, connector) = ready.DataSources[id];
      return current is null
         ? new ConnectorDefinition(connector.Key, connector.Name, connector.Type, plugin)
         : current with { Name = connector.Name, Kind = connector.Type };
    }
  }
}
