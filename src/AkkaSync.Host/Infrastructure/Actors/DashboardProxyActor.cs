using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Host.Application.Dashboard.EventMappers;
using AkkaSync.Host.Application.Dashboard.Stores;
using AkkaSync.Host.Application.HostState;
using AkkaSync.Host.Application.Messaging;
using AkkaSync.Host.Domain.Dashboard.Repositories;

namespace AkkaSync.Host.Infrastructure.Actors;

public class DashboardProxyActor : ReceiveActor
{
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private readonly IHostStateStore _stateStore;
  // private readonly IDashboardEventStore _eventStore;
  private readonly IEventEnvelopeFactory _factory;
  private readonly IEventEnvelopePublisher _publisher;
  public DashboardProxyActor(
    IHostStateStore stateStore, 
    /*IDashboardEventStore eventStore,*/
    IEventEnvelopeFactory factory,
    IEventEnvelopePublisher publisher)
  {
    _stateStore = stateStore;
    // _eventStore = eventStore;
    _factory = factory;
    _publisher = publisher;

    ReceiveAsync<ISyncEvent>(async @event =>
    {
      var envelopes = new List<EventEnvelope<IDashboardEvent>>();
      var current = _stateStore.Snapshot;
      var next = SnapshotEventMapper.TryMap(current, @event);
      if(!ReferenceEquals(current, next))
      {
        _stateStore.Update(next);
        var payload = DashboardEventMapper.TryMap(next);
        var envelope = _factory.Create(payload.TypeName, payload, DateTimeOffset.UtcNow);
        envelopes.Add(envelope);
      }
      var dashboardEvent = RecentEventMapper.TryMap(@event);
      if(dashboardEvent != null)
      {
        // _eventStore.Append(dashboardEvent);
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
