using System;
using System.Text.Json;

namespace AkkaSync.Host.Application.Query;

public class DashboardQueryDispatcher : IDashboardQueryDispatcher
{
  private readonly IDictionary<string, IQueryHandler> _handlers;
  public DashboardQueryDispatcher(IEnumerable<IQueryHandler> handlers)
  {
    _handlers = handlers.ToDictionary(h => h.Type);
  }
  public Task<JsonElement> DispatchAsync(QueryEnvelope query)
  {
    if(!_handlers.TryGetValue(query.Method, out var handler))
    {
      throw new InvalidOperationException($"No handler registered for query type {query.Method}");
    }
    return handler.HandleAsync(query.Payload);
  }
}
