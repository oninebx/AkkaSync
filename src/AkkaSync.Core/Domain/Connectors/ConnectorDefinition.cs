using AkkaSync.Abstractions;

namespace AkkaSync.Core.Domain.DataSources
{
  public sealed record ConnectorDefinition(string Key, string Name, string Kind, string Plugin) : ISnapshot
  {
    public string Identifier => Key;
  }
}
