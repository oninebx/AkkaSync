using System;
using System.Text.Json;
using AkkaSync.Host.Application.Messaging;

namespace AkkaSync.Host.Application.Dashboard.Events;

public record QueryEventTested(JsonElement Data) : IDashboardEvent
{
  public string TypeName => "query.event.tested";

  public object Payload => Data;
}
