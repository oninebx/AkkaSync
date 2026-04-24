using Akka.Actor;

namespace AkkaSync.Infrastructure.Abstractions
{
  public interface ISyncActorResolver
  {
    IActorRef ActorOf<TActor>(IActorContext context, string? name = null, params object[] args) where TActor : ActorBase;
  }
}
