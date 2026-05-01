using AkkaSync.Infrastructure.Abstractions;
using AkkaSync.Infrastructure.Messaging.Publish;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Infrastructure.Realtime
{
  public class SignalRPublisher : IEnvelopePublisher
  {
    private readonly IHubContext<StateHub> _hub;
    private readonly IHubCallerRegistry _registry;

    public SignalRPublisher(IHubContext<StateHub> hub, IHubCallerRegistry registry) 
    {
      _hub = hub;
      _registry = registry;
    }

    public async Task PublishAsync(PatchEnvelope envelope)
    {
      var clients = _registry.GetClients();
      foreach (var clientId in clients)
      {
        await _hub.Clients.Client(clientId).SendAsync("ReceivePatch", envelope);
      }
    }
  }
}
