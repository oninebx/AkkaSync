using System;
using System.Collections.Concurrent;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Web;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Host.Infrastructure.SignalR;

public class SignalREventEnvelopePublisher : IEventEnvelopePublisher
{
  private readonly IHubContext<DashboardHub> _hub;
  private readonly IDashboardClientRegistry _registry;

  public SignalREventEnvelopePublisher(IHubContext<DashboardHub> hub, IDashboardClientRegistry registry)
  {
    _hub = hub;
    _registry = registry;
  }

  public async Task PublishAsync<T>(EventEnvelope<T> envelope) where T : IDashboardEvent
  {
    var clients = _registry.GetClientsForEnvelope(envelope);
    foreach(var clientId in clients)
    {
      await _hub.Clients.Client(clientId).SendAsync("ReceiveDashboardEvent", new List<EventEnvelope<T>> { envelope });
      _registry.UpdateLastSeen(clientId, envelope.Sequence);
    }
  }

  // public async Task NotifyAsync(EventEnvelope envelope)
  // {
  //   foreach(var state in _connections.Values)
  //   {
  //     if(envelope.Sequence > state.LastSeenSequence)
  //     {
  //       await _hub.Clients.Client(state.ConnectionId).SendAsync("EventEnvelope", new List<EventEnvelope>{ envelope });
  //       state.LastSeenSequence = envelope.Sequence;
  //     }
  //   }
  // }

  // public Task RegisterClientAsync(string connectionId, long lastSeenSequence)
  // {
  //   var minSeq = _store.GetMinSequence();
  //   IReadOnlyList<EventEnvelope> events;
  //   if(lastSeenSequence > 0 && minSeq.HasValue && lastSeenSequence < minSeq)
  //   {
  //     // TODO: Notifies the client of the gap between its last event and the current event.
  //     events = _store.GetLatest();
  //   }
  //   else
  //   {
  //     events = lastSeenSequence > 0 ? _store.GetAfter(lastSeenSequence) : _store.GetLatest();
  //   }
  //   var lastSequence = events.LastOrDefault()?.Sequence ?? lastSeenSequence;
  //   _connections[connectionId] = new DashboardClientState(connectionId, lastSequence);
  //   return _hub.Clients.Client(connectionId).SendAsync("EventEnvelope", events);
  // }

  // public void UnregisterClient(string connectionId) => _connections.TryRemove(connectionId, out _);
}
