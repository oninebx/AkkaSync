using Akka.Actor;

namespace AkkaSync.Infrastructure.Abstractions
{
  public interface ISyncActorRegistry
  {
    IActorRef Get<TActor>();
    void Register<TActor>(IActorRef actor);
  }
}
