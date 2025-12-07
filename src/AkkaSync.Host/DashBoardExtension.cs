using System;
using Akka.Actor;
using AkkaSync.Host.Actors;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Host;

public static class DashBoardExtension
{
  public static IServiceProvider RunDashboard(this IServiceProvider services, ActorSystem actorSystem)
  {
    var hubContext = services.GetRequiredService<IHubContext<DashboardHub>>();
    actorSystem.ActorOf(Props.Create(() => new DashboardProxyActor(hubContext)), "dashboard-proxy");
    return services;
  }
}
