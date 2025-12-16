using System;
using Akka.Actor;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Host.Web;

public class DashboardHub : Hub
{
  private IHostStateStore _store;
  
  public DashboardHub(IHostStateStore store)
  {
    _store = store;
  }

  public Task<HostSnapshot> GetHostSnapshot()
  {
    return Task.FromResult(_store.GetSnapshot());
  }
}
