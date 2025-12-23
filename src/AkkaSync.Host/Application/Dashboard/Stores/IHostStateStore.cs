using System;
using AkkaSync.Host.Domain.Entities;

namespace AkkaSync.Host.Application.Dashboard.Stores;

public interface IHostStateStore
{
  HostSnapshot Snapshot { get; }
  void Update(HostSnapshot snapshot);
  event Func<HostSnapshot, Task>? OnUpdated;
}
