using System;
using System.Collections.Concurrent;
using Akka.Actor;
using AkkaSync.Host.Application.Dashboard.Events;
using AkkaSync.Host.Application.Dashboard.Publishers;
using AkkaSync.Host.Application.Dashboard.Stores;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Host.Web;

public class DashboardHub : Hub
{
  private readonly IHostSnapshotPublisher _snapshotPublisher;
  private readonly IEventEnvelopePublisher _envelopePublisher;
  
  public DashboardHub(IHostSnapshotPublisher snapshotPublisher, IEventEnvelopePublisher envelopePublisher)
  {
    _snapshotPublisher = snapshotPublisher;
    _envelopePublisher = envelopePublisher;
  }

  public override Task OnConnectedAsync()
  {
    _snapshotPublisher.PublishCurrentAsync(Context.ConnectionId);

    var lastSeenSeq = GetLastSeenSeq();
    _envelopePublisher.RegisterClientAsync(Context.ConnectionId, lastSeenSeq);
    return base.OnConnectedAsync();
  }

  public override Task OnDisconnectedAsync(Exception? exception)
  {
    _envelopePublisher.UnregisterClient(Context.ConnectionId);
    return base.OnDisconnectedAsync(exception);
  }

  private long GetLastSeenSeq()
  {
    if(Context.GetHttpContext()?.Request.Headers.TryGetValue("x-last-seq", out var values) == true &&
    long.TryParse(values.First(), out var seq))
    {
      return seq;
    }
    return 0;
  }
}
