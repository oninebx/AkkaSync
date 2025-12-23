using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Host.Application.Dashboard.Events;
using AkkaSync.Host.Application.Dashboard.Stores;
using AkkaSync.Host.Domain.States;

namespace AkkaSync.Host.Infrastructure.Actors;

public class DashboardProxyActor : ReceiveActor
{
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private readonly IHostStateStore _stateStore;
  private readonly IDashboardEventStore _eventStore;
  public DashboardProxyActor(IHostStateStore stateStore, IDashboardEventStore eventStore)
  {
    _stateStore = stateStore;
    _eventStore = eventStore;

    Receive<ISyncEvent>(evt =>
    {
      var current = _stateStore.Snapshot;
      var next = HostStateReducer.Reduce(current, evt);
      if(!ReferenceEquals(current, next))
      {
        _stateStore.Update(next);
      }
      var dashboardEvent = DashboardEventMapper.TryMap(evt);
      if(dashboardEvent != null)
      {
        _eventStore.Append(dashboardEvent);
      }
    });

  }

  protected override void PreStart()
  {
    Context.System.EventStream.Subscribe(Self, typeof(ISyncEvent));
  }

  protected override void PostStop()
  {
    Context.System.EventStream.Unsubscribe(Self, typeof(ISyncEvent));
  }
}
