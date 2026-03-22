using System.Text.Json;
using AkkaSync.Abstractions;

namespace AkkaSync.Host.Application.Query.Mapper
{
  public class RequestQueryMapper : IRequestQueryMapper
  {
    private readonly Dictionary<string, Type> _queryTypes;

    public RequestQueryMapper()
    {
      _queryTypes = AppDomain.CurrentDomain.GetAssemblies()
           .SelectMany(a => {
             try
             {
               return a.GetTypes();
             }
             catch
             {
               return [];
             }
             
           })
           .Where(t => typeof(IRequestQuery).IsAssignableFrom(t) && !t.IsInterface)
           .ToDictionary(t => t.Name, t => t);
    }
    public IRequestQuery? Map(QueryEnvelope envelope)
    {
      if (!_queryTypes.TryGetValue(envelope.Method, out var type))
      {
        return null;
      }

      return (IRequestQuery)JsonSerializer.Deserialize(envelope.Payload, type)!;
    }
  }
}
