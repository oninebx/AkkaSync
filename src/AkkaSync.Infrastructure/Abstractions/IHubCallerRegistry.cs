using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Infrastructure.Abstractions
{
  public interface IHubCallerRegistry
  {
    void RegisterCaller(string connectionId);
    void UnregisterCaller(string connectionId);
    IEnumerable<string> GetClients();
  }
}
