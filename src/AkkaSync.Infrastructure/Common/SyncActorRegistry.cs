using Akka.Actor;
using Akka.Hosting;
using AkkaSync.Core.Common;

namespace AkkaSync.Infrastructure.Common
{
  public sealed class SyncActorRegistry : ISyncActorRegistry
  {
    private readonly IActorRegistry _registry;
    public SyncActorRegistry(ActorRegistry registry) 
    {
      _registry = registry;
    }
    public IActorRef Get<TActor>() => _registry.Get<TActor>();

    public void Register<TActor>(IActorRef actor) => _registry.Register<TActor>(actor);
  }
}
