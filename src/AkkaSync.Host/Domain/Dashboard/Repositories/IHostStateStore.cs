using System;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Domain.Dashboard.Repositories;

public interface IHostStateStore
{
  HostSnapshot Snapshot { get; }
  void Update(HostSnapshot snapshot);
}
