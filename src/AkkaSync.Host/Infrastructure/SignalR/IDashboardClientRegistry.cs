using System;
using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Host.Infrastructure.SignalR;

public interface IDashboardClientRegistry
{
  void RegisterClientAsync(string connectionId, long lastSeenSequence);
  void UnregisterClient(string connectionId);
  IEnumerable<string> GetClientsForEnvelope(EventEnvelope envelope);
  void UpdateLastSeen(string connectionId, long sequence);
}
