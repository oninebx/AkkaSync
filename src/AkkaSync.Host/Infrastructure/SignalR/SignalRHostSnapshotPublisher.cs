using System;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Domain.Entities;
using AkkaSync.Host.Web;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Host.Infrastructure.SignalR;

public class SignalRHostSnapshotPublisher : IHostSnapshotPublisher
{
  private readonly IHubContext<DashboardHub> _hub;
  private readonly ILogger<SignalRHostSnapshotPublisher> _logger;

   public SignalRHostSnapshotPublisher(IHubContext<DashboardHub> hub, ILogger<SignalRHostSnapshotPublisher> logger)
  {
    _hub = hub;
    _logger = logger;
  }

  public Task BroadcastSnapshot(HostSnapshot snapshot)
  {
    return _hub.Clients.All.SendAsync("HostSnapshot", snapshot);
  }
}
