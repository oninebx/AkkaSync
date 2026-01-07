using System;
using System.Collections.Concurrent;
using AkkaSync.Host.Application.Messaging;

namespace AkkaSync.Host.Infrastructure.SignalR;

public class DashboardClientRegistry : IDashboardClientRegistry
{
  private readonly ConcurrentDictionary<string, DashboardClientState> _clients = [];

  public DashboardClientRegistry()
  {
    
  }

  public IEnumerable<string> GetClientsForEnvelope(EventEnvelope envelope) => 
    _clients.Where(kv => kv.Value.LastSeenSequence < envelope.Sequence)
            .Select(kv => kv.Key);

  public void RegisterClientAsync(string connectionId, long lastSeenSequence) => _clients[connectionId] = new DashboardClientState(connectionId, lastSeenSequence);

  public void UnregisterClient(string connectionId) => _clients.TryRemove(connectionId, out _);

  public void UpdateLastSeen(string connectionId, long sequence) => _clients[connectionId].LastSeenSequence = sequence;

}
