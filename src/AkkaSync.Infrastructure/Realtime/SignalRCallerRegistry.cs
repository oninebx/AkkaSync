using AkkaSync.Infrastructure.Abstractions;
using System.Collections.Concurrent;

namespace AkkaSync.Infrastructure.Realtime
{
  public class SignalRCallerRegistry : IHubCallerRegistry
  {
    private readonly ConcurrentDictionary<string, byte> _clients = [];

    public SignalRCallerRegistry() { }
    public IEnumerable<string> GetClients() => _clients.Keys;

    public void RegisterCaller(string connectionId) => _clients.TryAdd(connectionId, 0);

    public void UnregisterCaller(string connectionId) => _clients.TryRemove(connectionId, out _);
  }
}
