using System;
using AkkaSync.Host.Application.Dashboard.Publishers;
using AkkaSync.Host.Application.Dashboard.Stores;
using AkkaSync.Host.Domain.Entities;
using AkkaSync.Host.Web;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Host.Infrastructure.SignalR;

public class SignalRHostSnapshotPublisher : IHostSnapshotPublisher
{
  private readonly IHubContext<DashboardHub> _hub;
  private readonly ILogger<SignalRHostSnapshotPublisher> _logger;
  private readonly IHostStateStore _store;

   public SignalRHostSnapshotPublisher(IHubContext<DashboardHub> hub, IHostStateStore store, ILogger<SignalRHostSnapshotPublisher> logger)
  {
    _hub = hub;
    _logger = logger;
    _store = store;
    store.OnUpdated += NotifyAsync;
  }

  public Task NotifyAsync(HostSnapshot snapshot)
  {
    return _hub.Clients.All.SendAsync("HostSnapshot", _store.Snapshot);
  }

  public Task PublishCurrentAsync(string connectionId)
  {
    return _hub.Clients.Client(connectionId).SendAsync("HostSnapshot", _store.Snapshot);
  }
}
