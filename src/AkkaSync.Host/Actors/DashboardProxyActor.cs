using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Core.Messging;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Host.Actors;

public class DashboardProxyActor : ReceiveActor
{
  private readonly IHubContext<DashboardHub> _hubContext;
  private readonly ILoggingAdapter _logger = Context.GetLogger();

  public DashboardProxyActor(IHubContext<DashboardHub> hubContext)
  {
    _hubContext = hubContext;
    Receive<DashboardEvent>(evt =>
    {
      _logger.Info($"[DashboardEvent] {evt.Type}: {evt.Data}");
      _hubContext.Clients.All.SendAsync("receiveEvent", evt);
    });
  }

  
}
