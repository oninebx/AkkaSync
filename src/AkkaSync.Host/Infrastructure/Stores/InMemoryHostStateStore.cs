using System;
using AkkaSync.Host.Application.Dashboard.Publishers;
using AkkaSync.Host.Application.Dashboard.Stores;
using AkkaSync.Host.Domain.Entities;

namespace AkkaSync.Host.Infrastructure.Stores;

public class InMemoryHostStateStore(ILogger<InMemoryHostStateStore> logger) : IHostStateStore
{
  private volatile HostSnapshot _snapshot = HostSnapshot.Empty;
  public HostSnapshot Snapshot => _snapshot;
  private readonly ILogger<InMemoryHostStateStore> _logger = logger;

  public event Func<HostSnapshot, Task>? OnUpdated;

  public void Update(HostSnapshot snapshot)
  {
   _snapshot = snapshot;
   OnUpdated?.Invoke(_snapshot);
  }
}
