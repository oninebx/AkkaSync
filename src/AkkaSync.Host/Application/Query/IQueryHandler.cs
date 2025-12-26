using System;
using System.Text.Json;

namespace AkkaSync.Host.Application.Query;

public interface IQueryHandler
{
  string Type { get; }
  Task<JsonElement> HandleAsync(JsonElement payload);
}
