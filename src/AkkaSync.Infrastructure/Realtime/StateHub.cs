using Akka.Actor;
using AkkaSync.Abstractions;
using AkkaSync.Core.Registries;
using AkkaSync.Infrastructure.Abstractions;
using AkkaSync.Infrastructure.Actors;
using AkkaSync.Infrastructure.Messaging.Publish;
using AkkaSync.Infrastructure.StateStore;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace AkkaSync.Infrastructure.Realtime
{
  public class StateHub: Hub
  {
    private readonly IHubCallerRegistry _registry;
    private readonly ISnapshotStore _store;
    private readonly ProjectionRegistry _projectionRegistry;
    private readonly IEnvelopeFactory _envelopeFactory;
    private readonly IEnvelopePublisher _publisher;
    private readonly IRequestQueryMapper _mapper;
    private readonly IActorRef _syncGateway;
    public StateHub(
      IHubCallerRegistry registry, 
      ISnapshotStore store, 
      ProjectionRegistry projectionRegistry, 
      IEnvelopeFactory envelopeFactory,
      IEnvelopePublisher publisher,
      IRequestQueryMapper mapper,
      ISyncActorRegistry actorRegistry) 
    {
      _registry = registry;
      _store = store;
      _projectionRegistry = projectionRegistry;
      _envelopeFactory = envelopeFactory;
      _publisher = publisher;
      _mapper = mapper;
      _syncGateway = actorRegistry.Get<SyncGatewayActor>();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
      _registry.UnregisterCaller(Context.ConnectionId);
      return base.OnDisconnectedAsync(exception);
    }

    public override async Task OnConnectedAsync()
    {
      var changesToSend = new List<IChangeSet>();
      _registry.RegisterCaller(Context.ConnectionId);
      var snapshots = _store.GetCurrent();
      foreach (var snapshot in snapshots)
      {
        if (_projectionRegistry.TryProjection(snapshot.Key, [], snapshot.Value, out var chagnes))
        {
          changesToSend.AddRange(chagnes);
        }
      }

      var envelope = _envelopeFactory.Create(changesToSend);
      await _publisher.PublishAsync(envelope);

    }

    public async Task<QueryResponse> Query(QueryEnvelope envelope)
    {
      var query = _mapper.Map(envelope);
      if (query is null)
      {
        return new QueryResponse(false, $"{envelope.Method} is not supported.");
      }

      _syncGateway.Tell(query);
      return new QueryResponse(true, $"{envelope.Method} is executing.");
    }
  }

  public sealed record QueryResponse(bool Success, string Message);

}
