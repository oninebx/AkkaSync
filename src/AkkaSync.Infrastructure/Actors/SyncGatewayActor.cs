using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Core.Actors;
using AkkaSync.Core.Common;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Infrastructure.Messaging.Publish;
using System;

namespace AkkaSync.Infrastructure.Actors;

public class SyncGatewayActor : ReceiveActor
{
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private IReadOnlyDictionary<Type, IActorRef>? _routes;

  private IActorRef _pipelineManager;
  private IActorRef _pluginManager;
  public SyncGatewayActor(
    ISyncActorRegistry actorRegistry,
    IDashboardStore store,
    IEventNotificationMapper notificationMapper,
    EventReducerRegistry reducerRegistry,
    IEventEnvelopeFactory factory,
    IEventEnvelopePublisher publisher)
  {

    _pipelineManager = actorRegistry.Get<PipelineManagerActor>();
    _pluginManager = actorRegistry.Get<PluginManagerActor>();
    var _routes = BuildQueryRoutes(_pipelineManager, _pluginManager);

    ReceiveAsync<INotificationEvent>(async @event =>
    {
      var envelopes = new List<EventEnvelope>();
      var storeValues = store.GetEventsToReplay(0);
      foreach(var current in storeValues)
      {
        if(reducerRegistry.TryReduce(current, @event, out var next) 
          && !ReferenceEquals(current, next))
        {
          _logger.Info("event {0} is emitted.", @event);
          store.Update(next);
          var payload = notificationMapper.TryMap(next, @event);
          if(payload is not null)
          {
            var envelope = factory.Create(payload.TypeName, payload, DateTimeOffset.UtcNow);
            envelopes.Add(envelope);
          }
        }
      }

      foreach(var envelop in envelopes)
      {
        await publisher.PublishAsync(envelop);
      }
    });

  }
  protected override void PreStart()
  {
    Context.System.EventStream.Subscribe(Self, typeof(INotificationEvent));
    _logger.Info("DashboardProxyActor started.");
  }

  protected override void PostStop()
  {
    Context.System.EventStream.Unsubscribe(Self, typeof(INotificationEvent));
  }

  private IReadOnlyDictionary<Type, IActorRef> BuildQueryRoutes(IActorRef pipeline, IActorRef plugin)
  {
    var routes = new Dictionary<Type, IActorRef>();
    return routes;
  }
}
