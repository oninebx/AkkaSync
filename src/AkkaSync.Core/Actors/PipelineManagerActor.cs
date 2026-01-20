using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Schedules;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Core.Notifications;

namespace AkkaSync.Core.Actors;

public class PipelineManagerActor : ReceiveActor
{
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private IActorRef? _schedulerActor;
  private IDictionary<string, Props> _props;
  private IReadOnlyList<PipelineInfo> _pipelines = [];
  private IReadOnlyDictionary<string, string> _schedules = new Dictionary<string, string>();
  public PipelineManagerActor(IDictionary<string, Props> props)
  {
    _props = props;
    Receive<PeerRegistered>(msg => HandlePeerRegistered(msg));
  }

  override protected void PreStart()
  {
    var strategy = new OneForOneStrategy(
      maxNrOfRetries: 3,
      withinTimeRange: TimeSpan.FromSeconds(10),
      localOnlyDecider: ex =>
      {
        return Directive.Restart;
      }
    );
    _schedulerActor = Context.ActorOf(_props["pipeline-scheduler"].WithSupervisorStrategy(strategy), "pipeline-scheduler");
    var registryActor = Context.ActorOf(_props["pipeline-registry"].WithSupervisorStrategy(strategy), "pipeline-registry");
    _schedulerActor.Tell(new SharedProtocol.RegisterPeer(registryActor));
    registryActor.Tell(new SharedProtocol.RegisterPeer(_schedulerActor));
  }

  private void HandlePeerRegistered(PeerRegistered msg)
  {
     _logger.Info("{0} actor is ready at {1}.", Self.Path.Name, DateTimeOffset.UtcNow);
      if(msg.Payload is IReadOnlyList<PipelineInfo> list)
      {
        _pipelines = list;
      }
      
      if(msg.Payload is IReadOnlyDictionary<string, string> dict)
      {
        _schedules = dict;
      }

      if(_pipelines.Count == 0)
      {
        _logger.Warning("No pipelines are registered in the system.");
        return;
      }
      if(_schedules.Count == 0)
      {
        _logger.Warning("No schedules are registered in the system.");
        return;
      }
      Context.System.EventStream.Publish(new SyncEngineReady(_pipelines, _schedules));
      _schedulerActor.Tell(new PipelineSchedulerProtocol.Start());
  }

  protected override void PostStop()
  {
    Context.System.EventStream.Publish(new SyncEngineStopped());
  }
}
