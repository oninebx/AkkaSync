using System;
using System.Collections.Concurrent;
using AkkaSync.Host.Application.Dashboard.Events;
using AkkaSync.Host.Application.Dashboard.Publishers;
using AkkaSync.Host.Application.Dashboard.Stores;
using AkkaSync.Host.Web;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Host.Infrastructure.SignalR;

public class SignalREventEnvelopePublisher : IEventEnvelopePublisher
{
  private readonly IHubContext<DashboardHub> _hub;
  private readonly IDashboardEventStore _store;
  private readonly ConcurrentDictionary<string, DashboardClientState> _connections = [];

  public SignalREventEnvelopePublisher(IHubContext<DashboardHub> hub, IDashboardEventStore store)
  {
    _hub = hub;
    _store = store;
    store.OnAppended += NotifyAsync;
  }

  public async Task NotifyAsync(EventEnvelope envelope)
  {
    foreach(var state in _connections.Values)
    {
      if(envelope.Sequence > state.LastSeenSequence)
      {
        await _hub.Clients.Client(state.ConnectionId).SendAsync("EventEnvelope", new List<EventEnvelope>{ envelope });
        state.LastSeenSequence = envelope.Sequence;
      }
    }
  }

  public Task RegisterClientAsync(string connectionId, long lastSeenSequence)
  {
    var minSeq = _store.GetMinSequence();
    IReadOnlyList<EventEnvelope> events;
    if(lastSeenSequence > 0 && minSeq.HasValue && lastSeenSequence < minSeq)
    {
      // TODO: Notifies the client of the gap between its last event and the current event.
      events = _store.GetLatest();
    }
    else
    {
      events = lastSeenSequence > 0 ? _store.GetAfter(lastSeenSequence) : _store.GetLatest();
    }
    var lastSequence = events.LastOrDefault()?.Sequence ?? lastSeenSequence;
    _connections[connectionId] = new DashboardClientState(connectionId, lastSequence);
    return _hub.Clients.Client(connectionId).SendAsync("EventEnvelope", events);
  }

  public void UnregisterClient(string connectionId) => _connections.TryRemove(connectionId, out _);
}
