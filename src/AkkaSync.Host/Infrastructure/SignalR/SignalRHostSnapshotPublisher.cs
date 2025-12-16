using System;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Host.Infrastructure.SignalR;

public class SignalRHostSnapshotPublisher : IHostSnapshotPublisher
{
  private readonly IEnumerable<IHubContext<Hub>> _hubs;

  public SignalRHostSnapshotPublisher(IEnumerable<IHubContext<Hub>> hubs)
  {
    _hubs = hubs;
  }

  public Task BroadcastSnapshot(HostSnapshot snapshot)
  {
    var tasks = _hubs.Select(hub => hub.Clients.All.SendAsync("HostSnapshot", snapshot));
    return Task.WhenAll(tasks);
  }
}
