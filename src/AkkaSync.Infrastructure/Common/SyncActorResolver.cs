using Akka.Actor;
using Akka.DependencyInjection;
using AkkaSync.Core.Common;
using AkkaSync.Infrastructure.Abstractions;

namespace AkkaSync.Infrastructure.Common
{
  public sealed class SyncActorResolver : ISyncActorResolver
  {
    private readonly ActorSystem _system;
    public SyncActorResolver(ActorSystem system) 
    {
      _system = system;
    }

    public IActorRef ActorOf<TActor>(IActorContext context, string? name = null, params object[] args) where TActor : ActorBase
    {
      var resolver = DependencyResolver.For(_system);
      var props = resolver.Props<TActor>(args);

      return name == null ? context.ActorOf(props) : context.ActorOf(props, name);
    }

  }
}
