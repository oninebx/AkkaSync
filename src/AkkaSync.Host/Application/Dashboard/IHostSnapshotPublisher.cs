using System;
using AkkaSync.Host.Domain.Entities;

namespace AkkaSync.Host.Application.Dashboard;

public interface IHostSnapshotPublisher
{
  Task BroadcastSnapshot(HostSnapshot snapshot);
}
