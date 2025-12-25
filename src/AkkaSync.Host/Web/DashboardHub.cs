using System;
using System.Collections.Concurrent;
using Akka.Actor;
using AkkaSync.Host.Application.Common;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;
using AkkaSync.Host.Infrastructure.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Host.Web;

public class DashboardHub : Hub
{
  private readonly IDashboardClientRegistry _registry;
  private readonly IEventEnvelopePublisher _envelopePublisher;
  private readonly IEnumerable<IReplayStore<IStoreValue>> _replayStores;
  private readonly IEventEnvelopeFactory _factory;
  
  public DashboardHub(
    IDashboardClientRegistry registry, 
    IEventEnvelopePublisher envelopePublisher,
    IEnumerable<IReplayStore<IStoreValue>> replayStores,
    IEventEnvelopeFactory factory
    )
  {
    _registry = registry;
    _envelopePublisher = envelopePublisher;
    _replayStores = replayStores;
    _factory = factory;
  }

  public override async Task OnConnectedAsync()
  {
    var lastSeenSeq = GetLastSeenSeq();
    _registry.RegisterClientAsync(Context.ConnectionId, lastSeenSeq);
    
    var eventsToSend = _replayStores
      .SelectMany(store => store.GetEventsToReplay(lastSeenSeq))
      .Select(value =>
      {
        var payload = DashboardEventMapper.TryMap(value);
        return _factory.Create(payload.TypeName, payload, DateTimeOffset.UtcNow);
      });
    foreach(var envelope in eventsToSend)
    {
      await _envelopePublisher.PublishAsync(envelope);
    }
  }

  public override Task OnDisconnectedAsync(Exception? exception)
  {
    _registry.UnregisterClient(Context.ConnectionId);
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
