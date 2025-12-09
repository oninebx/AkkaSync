using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Core.Messging;
using AkkaSync.Host.Messaging;
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
    ReceiveAsync<HostOnline>(async msg =>
    {
      _logger.Info($"[HostOnline] Host came online at {msg.Timestamp}.");
      await _hubContext.Clients.All.SendAsync("hostOnline", new
      {
        online = true,
        timestamp = msg.Timestamp,
        version = msg.HostVersion,
        machineName = msg.MachineName
      });
    });
    ReceiveAsync<HostOffline>(async msg =>
    {
      _logger.Info($"[HostOffline] Host went offline at {msg.Timestamp}.");
      await _hubContext.Clients.All.SendAsync("hostOffline", new 
      { 
        online = false, 
        timestamp = msg.Timestamp, 
        reason = msg.Reason 
      });
    });
  }

  protected override void PreStart()
  {
    Context.System.EventStream.Subscribe(Self, typeof(HostOnline));
    Context.System.EventStream.Subscribe(Self, typeof(HostOffline));
  }

  protected override void PostStop()
  {
    Context.System.EventStream.Unsubscribe(Self, typeof(HostOnline));
    Context.System.EventStream.Unsubscribe(Self, typeof(HostOffline));
  }
  
}
