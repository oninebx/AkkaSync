using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Core.Actors;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Infrastructure;

public record ActorHook(Props Props, string Name);

public class SyncRuntimeActor : ReceiveActor
{
  private readonly IEnumerable<ActorHook> _hooks;
  private readonly IDictionary<string, Props> _props;
  private ILoggingAdapter _logger = Context.GetLogger();
  
  public SyncRuntimeActor(IEnumerable<ActorHook> hooks, IDictionary<string, Props> props)
  {
    _hooks = hooks;
    _props = props;
    Receive<Terminated>(t =>
    {
      _logger.Info("{0} actor terminated at {1}", t.ActorRef.Path.Name, DateTimeOffset.UtcNow);
    });
  }

  protected override void PreStart()
  {
    var strategy = new OneForOneStrategy(
      maxNrOfRetries: 3,
      withinTimeRange: TimeSpan.FromSeconds(10),
      localOnlyDecider: ex =>
      {
        return Directive.Restart;
      }
    );

    var managerActor = Context.ActorOf(Props.Create<PipelineManagerActor>(_props).WithSupervisorStrategy(strategy), "pipeline-manager");

    IActorRef? dashboardActor = null;
    foreach(var hook in _hooks)
    {
      var actorRef = Context.ActorOf(hook.Props.WithSupervisorStrategy(strategy), hook.Name);
      Context.Watch(actorRef);
      switch(hook.Name) {
        case "dashboard-proxy":
          dashboardActor = actorRef;
          break;
      }
    }
  
    if (dashboardActor == null)
    {
      throw new InvalidOperationException("DashboardProxy actor is not initialized.");
    }

    dashboardActor.Tell(new SharedProtocol.RegisterPeer(managerActor));
  }
}
