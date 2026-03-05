using System;
using System.Text.Json;

namespace AkkaSync.Host.Application.Query;

public interface IDashboardQueryDispatcher
{
  Task<JsonElement> DispatchAsync(QueryEnvelope query);
}
