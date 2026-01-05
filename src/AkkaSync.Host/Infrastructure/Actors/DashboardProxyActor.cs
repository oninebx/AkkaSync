using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Core.Runtime.PipelineScheduler;
using AkkaSync.Host.Application.Dashboard.Events;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Domain.Dashboard.Repositories;

namespace AkkaSync.Host.Infrastructure.Actors;

public class DashboardProxyActor : ReceiveActor
{
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private readonly IDashboardStore _store;
  private readonly IEventEnvelopeFactory _factory;
  private readonly IEventEnvelopePublisher _publisher;
  private readonly EventReducerRegistry _reducerRegistry;
  public DashboardProxyActor(
    IDashboardStore store,
    EventReducerRegistry reducerRegistry,
    IEventEnvelopeFactory factory,
    IEventEnvelopePublisher publisher)
  {
    _store = store;
    _reducerRegistry = reducerRegistry;
    _factory = factory;
    _publisher = publisher;

    ReceiveAsync<ISyncEvent>(async @event =>
    {
      
      if(@event is SchedulerStarted){
        _logger.Info("Received {0}", @event.GetType().Name);
      }
      var envelopes = new List<EventEnvelope<IDashboardEvent>>();
      var storeValues = _store.GetEventsToReplay(0);
      foreach(var current in storeValues)
      {
        if(reducerRegistry.TryReduce(current, @event, out var next) 
          && !ReferenceEquals(current, next))
        {
          _store.Update(next);
          var payload = DashboardEventMapper.TryMap(next);
          var envelope = _factory.Create(payload.TypeName, payload, DateTimeOffset.UtcNow);
          envelopes.Add(envelope);
        }
      }

      foreach(var envelop in envelopes)
      {
        await _publisher.PublishAsync(envelop);
      }
    });

  }
  protected override void PreStart()
  {
    Context.System.EventStream.Subscribe(Self, typeof(ISyncEvent));
    _logger.Error("DashboardProxyActor is starting.");
  }

  protected override void PostStop()
  {
    Context.System.EventStream.Unsubscribe(Self, typeof(ISyncEvent));
  }
}
