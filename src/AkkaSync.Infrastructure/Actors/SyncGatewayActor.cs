using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Infrastructure.Abstractions;
using AkkaSync.Infrastructure.Messaging.Publish;
using static AkkaSync.Infrastructure.Messaging.Contract.Update.Request;

namespace AkkaSync.Infrastructure.Actors;

public class SyncGatewayActor : ReceiveActor
{
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private IReadOnlyDictionary<Type, IActorRef>? _routes;

  private IActorRef? _pipelineManager;
  private IActorRef? _pluginManager;
  private ISyncActorRegistry _actorRegistry;
  public SyncGatewayActor(
    ISyncActorRegistry actorRegistry,
    IDashboardStore store,
    IEventNotificationMapper notificationMapper,
    EventReducerRegistry reducerRegistry,
    IEventEnvelopeFactory factory,
    IEventEnvelopePublisher publisher)
  {
    _actorRegistry = actorRegistry;
   

    ReceiveAsync<IProjectionEvent>(async @event =>
    {
      var envelopes = new List<EventEnvelope>();
      var storeValues = store.GetEventsToReplay(0);
      foreach(var current in storeValues)
      {
        try 
        {
          if (reducerRegistry.TryReduce(current, @event, out var next)
          && !ReferenceEquals(current, next))
          {
            _logger.Info("event {0} is emitted.", @event);
            store.Update(next);
            var payload = notificationMapper.TryMap(next, @event);
            if (payload is not null)
            {
              var envelope = factory.Create(payload.TypeName, payload, DateTimeOffset.UtcNow);
              envelopes.Add(envelope);
            }
          }
          else
          {
            _logger.Debug("event {0} is emmited without reducing.", @event);
          }
        }
        catch (Exception ex) 
        {
          _logger.Error(ex,"Reducer failed. Event={Event}, StateType={StateType}", @event.GetType().Name, current.GetType().Name);
        }

      }

      foreach(var envelop in envelopes)
      {
        await publisher.PublishAsync(envelop);
      }
    });

    Receive<IRequestQuery>(query => HandleQuery(query));
  }
  protected override void PreStart()
  {

    _pipelineManager = _actorRegistry.Get<PipelineManagerActor>();
    _pluginManager = _actorRegistry.Get<PluginManagerActor>();
    _routes = BuildQueryRoutes(_pipelineManager, _pluginManager);

    Context.System.EventStream.Subscribe(Self, typeof(IProjectionEvent));
    _logger.Info("SyncGatewayActor started.");
  }

  protected override void PostStop()
  {
    Context.System.EventStream.Unsubscribe(Self, typeof(IProjectionEvent));
  }

  private void HandleQuery(IRequestQuery msg)
  {
    if(_routes!.TryGetValue(msg.GetType(), out var target))
    {
      target.Forward(msg);
    }
    else
    {
      _logger.Warning("No route matches {0}", msg.GetType().Name);
    }
  }

  private IReadOnlyDictionary<Type, IActorRef> BuildQueryRoutes(IActorRef pipeline, IActorRef plugin)
  {
    var routes = new Dictionary<Type, IActorRef>()
    {
      { typeof(CheckVersions), plugin },
      { typeof(UpdatePlugin), plugin },
    };
    
    return routes;
  }
}
