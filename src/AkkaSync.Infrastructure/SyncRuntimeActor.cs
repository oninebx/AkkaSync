using System;
using System.Collections.Immutable;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Core.Actors;
using AkkaSync.Core.Commands.PipelineManager;
using AkkaSync.Core.Events;

namespace AkkaSync.Infrastructure;

public record ActorHook(Props Props, string Name);

public class SyncRuntimeActor : ReceiveActor
{
  private readonly IEnumerable<ActorHook> _hooks;
  private IActorRef? _pipelineManager;
  private ILoggingAdapter _logger = Context.GetLogger();
  
  public SyncRuntimeActor(IEnumerable<ActorHook> hooks)
  {
    _hooks = hooks;

    Receive<Terminated>(t =>
    {
      _logger.Info("{0} actor terminated at {1}", t.ActorRef.Path.Name, DateTimeOffset.UtcNow);
      if (t.ActorRef.Equals(_pipelineManager))
        Context.System.EventStream.Publish(new PipelineManagerFailed());
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
    foreach(var hook in _hooks)
    {
      var actorRef = Context.ActorOf(hook.Props.WithSupervisorStrategy(strategy), hook.Name);
      Context.Watch(actorRef);
      if(hook.Name == "pipeline-manager")
      {
        _pipelineManager = actorRef;
         _pipelineManager?.Tell(new StartPipelineManager());
      }
    }
  }
}
