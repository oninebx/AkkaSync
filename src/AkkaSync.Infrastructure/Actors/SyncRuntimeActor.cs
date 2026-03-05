using Akka.Actor;
using Akka.DependencyInjection;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Core.Actors;
using AkkaSync.Core.Common;

namespace AkkaSync.Infrastructure.Actors;

public record ActorHook(Props Props, string Name);

public class SyncRuntimeActor : ReceiveActor
{
  private ILoggingAdapter _logger = Context.GetLogger();
  

  public SyncRuntimeActor(ISyncActorRegistry registry)
  {
    var strategy = new OneForOneStrategy(
      maxNrOfRetries: 3,
      withinTimeRange: TimeSpan.FromSeconds(10),
      localOnlyDecider: ex =>
      {
        return Directive.Restart;
      }
    );

    var resolver = DependencyResolver.For(Context.System);
    var gatewayProps = resolver.Props<SyncGatewayActor>().WithSupervisorStrategy(strategy);
    var gateway = Context.ActorOf(gatewayProps, "sync-gateway");
    registry.Register<SyncGatewayActor>(gateway);
    
    var pipelineManagerActor = Context.ActorOf(resolver.Props<PipelineManagerActor>(new Dictionary<string, Props>
          {
            { "pipeline-registry", resolver.Props<PipelineRegistryActor>() },
            { "pipeline-scheduler", resolver.Props<PipelineSchedulerActor>() },
          }).WithSupervisorStrategy(strategy), "pipeline-manager");
    registry.Register<PipelineManagerActor>(pipelineManagerActor);

    var pluginManagerActor = Context.ActorOf(resolver.Props<PluginManagerActor>().WithSupervisorStrategy(strategy), "plugin-manager");
    registry.Register<PluginManagerActor>(pluginManagerActor);

    Receive<IRequestQuery>(msg =>
    {
      if(!gateway.IsNobody())
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
}
