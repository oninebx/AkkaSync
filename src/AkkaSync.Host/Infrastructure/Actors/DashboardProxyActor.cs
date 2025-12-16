using System;
using Akka.Actor;
using Akka.Event;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Domain.Entities;
using AkkaSync.Host.Domain.Events;
using AkkaSync.Host.Domain.Host.Queries;

namespace AkkaSync.Host.Actors;

public class DashboardProxyActor : ReceiveActor
{
  private readonly IHostSnapshotPublisher _publisher;
  private readonly IHostStateStore _store;
   private HostSnapshot _snapshot;
  private readonly ILoggingAdapter _logger = Context.GetLogger();

  public DashboardProxyActor(IHostSnapshotPublisher publisher, IHostStateStore store)
  {
    _publisher = publisher;
    _store = store;
    _snapshot = _store.GetSnapshot();

    Become(Offline);

  }

  protected override void PreStart()
  {
    Context.System.EventStream.Subscribe(Self, typeof(HostOnline));
    Context.System.EventStream.Subscribe(Self, typeof(HostOffline));

    Context.System.EventStream.Publish(new HostOnline());
  }

  protected override void PostStop()
  {
    Context.System.EventStream.Unsubscribe(Self, typeof(HostOnline));
    Context.System.EventStream.Unsubscribe(Self, typeof(HostOffline));

    Context.System.EventStream.Publish(new HostOffline());
  }

  private void Offline()
  {
    Receive<HostOnline>(_ =>
    {
      _snapshot = _snapshot with { Status = HostStatus.Online, Timestamp = DateTimeOffset.UtcNow };
      _store.Update(_snapshot);
      _publisher.BroadcastSnapshot(_snapshot);
      Become(Online);
      _logger.Info("Host is now ONLINE. Broadcasting snapshot...");
    });
  }

  private void Online()
  {
    Receive<HostOffline>(_ =>
    {
      _snapshot = _snapshot with { Status = HostStatus.Offline, Timestamp = DateTimeOffset.UtcNow };
      _store.Update(_snapshot);
      _publisher.BroadcastSnapshot(_snapshot);
      Become(Offline);
      _logger.Info("Host is now OFFLINE. Broadcasting snapshot...");
    });

    Receive<GetHostSnapshot>(_ =>
    {
      _logger.Info("Received GetHostSnapshot from {0}. Current status = {1}", Sender.Path, _snapshot.Status);

      Sender.Tell(_snapshot);
    });
  }
  
}
