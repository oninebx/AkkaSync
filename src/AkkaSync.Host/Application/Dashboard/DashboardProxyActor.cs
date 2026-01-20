using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Shared;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Application.Messaging;

namespace AkkaSync.Host.Application.Dashboard;

public class DashboardProxyActor : ReceiveActor
{
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private IActorRef? _pipelineManager;
  // private readonly IDashboardStore _store;
  // private readonly IEventEnvelopeFactory _factory;
  // private readonly IEventEnvelopePublisher _publisher;
  // private readonly EventReducerRegistry _reducerRegistry;
  public DashboardProxyActor(
    IDashboardStore store,
    EventReducerRegistry reducerRegistry,
    IEventEnvelopeFactory factory,
    IEventEnvelopePublisher publisher)
  {
    // _store = store;
    // _reducerRegistry = reducerRegistry;
    // _factory = factory;
    // _publisher = publisher;

    ReceiveAsync<INotificationEvent>(async @event =>
    {
      var envelopes = new List<EventEnvelope>();
      var storeValues = store.GetEventsToReplay(0);
      foreach(var current in storeValues)
      {
        if(reducerRegistry.TryReduce(current, @event, out var next) 
          && !ReferenceEquals(current, next))
        {
          _logger.Error("event {0} is emitted.", @event);
          store.Update(next);
          var payload = DashboardEventMapper.TryMap(next, @event);
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

    Receive<SharedProtocol.RegisterPeer>(msg => {
      _pipelineManager = msg.PeerRef;
      _logger.Info("Peer actor({0}) registered with Dashboard Proxy.", _pipelineManager.Path.Name);
    });

  }
  protected override void PreStart()
  {
    Context.System.EventStream.Subscribe(Self, typeof(INotificationEvent));
    _logger.Error("DashboardProxyActor is starting.");
  }

  protected override void PostStop()
  {
    Context.System.EventStream.Unsubscribe(Self, typeof(INotificationEvent));
  }
}
