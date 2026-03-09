using System;
using System.Collections.Concurrent;
using System.Text.Json;
using Akka.Actor;
using Akka.Hosting;
using AkkaSync.Core.Common;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Application.Query;
using AkkaSync.Host.Application.Query.Mapper;
using AkkaSync.Host.Infrastructure.SignalR;
using AkkaSync.Infrastructure.Actors;
using AkkaSync.Infrastructure.Messaging.Publish;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Host.Web;

public class DashboardHub : Hub
{
  private readonly IDashboardClientRegistry _registry;
  private readonly IEventEnvelopePublisher _envelopePublisher;
  private readonly IEventEnvelopeFactory _factory;
  private readonly IDashboardQueryDispatcher _dispatcher;
  private readonly IDashboardStore _store;

  private readonly IEventNotificationMapper _notificationMapper;
  private readonly IRequestQueryMapper _mapper;
  private readonly IActorRef _syncGateway;
  
  public DashboardHub(
    IEventNotificationMapper notificationMapper,
    IDashboardClientRegistry registry, 
    IEventEnvelopePublisher envelopePublisher,
    IDashboardStore store,
    IEventEnvelopeFactory factory,
    IDashboardQueryDispatcher dispatcher,
    IRequestQueryMapper mapper,
    ISyncActorRegistry actorRegistry
    )
  {
    _registry = registry;
    _envelopePublisher = envelopePublisher;
    _store = store;
    _factory = factory;
    _dispatcher = dispatcher;

    _notificationMapper = notificationMapper;
    _mapper = mapper;
    _syncGateway = actorRegistry.Get<SyncGatewayActor>();
  }

  public async Task<QueryResponse> Query(QueryEnvelope envelope)
  {
    //var resultJson = await _dispatcher.DispatchAsync(query);

    //if(query.ReturnImmediately)
    //{
    //  return resultJson;
    //}

    //var payload = new DashboardEvent("query.event.tested", resultJson);
    //var envelope = _factory.Create(payload.TypeName, payload, DateTimeOffset.UtcNow);
    //await _envelopePublisher.PublishAsync(envelope);

    var query = _mapper.Map(envelope);
    if(query is null)
    {
      return new QueryResponse(false, $"{envelope.Method} is not supported.");
    }

    _syncGateway.Tell(query);
    return new QueryResponse(true, $"{envelope.Method} is executing.");

  }

  public override async Task OnConnectedAsync()
  {
    var lastSeenSeq = GetLastSeenSeq();
    _registry.RegisterClientAsync(Context.ConnectionId, lastSeenSeq);
    
    var eventsToSend = _store.GetEventsToReplay(lastSeenSeq)
    .Select(value => _notificationMapper.TryMap(value, new DashboardInitialized()))
    .Where(payload => payload is not null)
    .Select(payload => _factory.Create(payload!.TypeName, payload, DateTimeOffset.UtcNow));

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
