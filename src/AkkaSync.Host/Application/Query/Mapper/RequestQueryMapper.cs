using System.Text.Json;

namespace AkkaSync.Host.Application.Query.Mapper
{
  public class RequestQueryMapper : IRequestQueryMapper
  {
    private readonly Dictionary<string, Type> _queryTypes;

    public RequestQueryMapper()
    {
      _queryTypes = AppDomain.CurrentDomain.GetAssemblies()
           .SelectMany(a => a.GetTypes())
           .Where(t => typeof(IRequestQuery).IsAssignableFrom(t) && !t.IsInterface)
           .ToDictionary(t => t.Name, t => t);
    }
    public IRequestQuery Map(QueryEnvelope envelope)
    {
      if (!_queryTypes.TryGetValue(envelope.Method, out var type))
      {
        throw new NotSupportedException(envelope.Method);
      }

      return (IRequestQuery)JsonSerializer.Deserialize(envelope.Payload, type)!;
    }
  }
}
