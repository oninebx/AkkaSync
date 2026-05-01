using AkkaSync.Abstractions;

namespace AkkaSync.Host.Application.DataSources.Changes
{
  public sealed record ConnectorDefinitionChangeSet(ChangeOperation Operation) : IChangeSet
  {
    public string Slice => "connectorConfig";

    public object Payload { get; set; } = default!;
  }
}
