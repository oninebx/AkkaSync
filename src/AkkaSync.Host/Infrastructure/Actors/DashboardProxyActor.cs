using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Core.Events;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Domain.States;
using AkkaSync.Host.Web;
using Microsoft.AspNetCore.SignalR;

namespace AkkaSync.Host.Infrastructure.Actors;

public class DashboardProxyActor : ReceiveActor
{
  private readonly ILoggingAdapter _logger = Context.GetLogger();
  private readonly IHostStateStore _store;
  public DashboardProxyActor(IHostStateStore store)
  {
    _store = store;
    Receive<ISyncEvent>(evt =>
    {
      var current = _store.Snapshot;
      var next = HostStateReducer.Reduce(current, evt);
      if(!ReferenceEquals(current, next))
      {
        _store.Update(next);
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
