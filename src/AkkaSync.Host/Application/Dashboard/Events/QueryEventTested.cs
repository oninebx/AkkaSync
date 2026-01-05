using System;
using System.Text.Json;
using AkkaSync.Host.Application.Messaging;

namespace AkkaSync.Host.Application.Dashboard.Events;

public record QueryEventTested : IDashboardEvent
{
  public QueryEventTested(JsonElement data)
  {
    Payload = data;
  }
  public string TypeName => "query.event.tested";

  public object Payload {get; init;} 

}
