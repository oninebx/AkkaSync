using System;
using AkkaSync.Host.Application.Messaging;

namespace AkkaSync.Host.Infrastructure.SignalR;

public interface IDashboardClientRegistry
{
  void RegisterClientAsync(string connectionId, long lastSeenSequence);
  void UnregisterClient(string connectionId);
  IEnumerable<string> GetClientsForEnvelope<T>(EventEnvelope<T> envelope) where T: IDashboardEvent;
  void UpdateLastSeen(string connectionId, long sequence);
}
