using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Infrastructure.Abstractions;

namespace AkkaSync.Infrastructure.Actors;

public record ActorHook(Props Props, string Name);

public class SyncRuntimeActor : ReceiveActor
{
  private ILoggingAdapter _logger = Context.GetLogger();
  

  public SyncRuntimeActor(ISyncActorRegistry registry, ISyncActorResolver resolver)
  {

    var gateway = resolver.ActorOf<SyncGatewayActor>(Context, "sync-gateway");
    Context.Watch(gateway);
    registry.Register<SyncGatewayActor>(gateway);

    var pipelineManagerActor = resolver.ActorOf<PipelineManagerActor>(Context, "pipeline-manager");
    Context.Watch(pipelineManagerActor);
    registry.Register<PipelineManagerActor>(pipelineManagerActor);

    var pluginManagerActor = resolver.ActorOf<PluginManagerActor>(Context, "plugin-manager");
    Context.Watch(pluginManagerActor);
    registry.Register<PluginManagerActor>(pluginManagerActor);

    Receive<IRequestQuery>(msg =>
    {
      if (!gateway.IsNobody())
      {
        gateway.Forward(msg);
      }
      else
      {
        _logger.Warning("SyncGatewayActor is invalid.");
      }

    });

    Receive<Terminated>(t =>
    {
      _logger.Info("{0} actor terminated at {1}", t.ActorRef.Path.Name, DateTimeOffset.UtcNow);
    });
  }

  protected override SupervisorStrategy SupervisorStrategy() => new OneForOneStrategy(
      maxNrOfRetries: 3,
      withinTimeRange: TimeSpan.FromSeconds(10),
      localOnlyDecider: ex =>
      {
        _logger.Error(ex, "An error occurred in child actor. Restarting the actor.");
        return Directive.Restart;
      }
    );
}
