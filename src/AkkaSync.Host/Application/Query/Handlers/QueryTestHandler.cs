using System;
using System.Text.Json;

namespace AkkaSync.Host.Application.Query.Handlers;

public record PingRequest(string Value);
public record PongResponse(string Value);
public class QueryTestHandler : IQueryHandler
{
  public string Type => "QueryTest";

  public async Task<JsonElement> HandleAsync(JsonElement payload)
  {
    var ping = payload.Deserialize<PingRequest>();
    return JsonSerializer.SerializeToElement(new PongResponse("Pong"));
  }
}
