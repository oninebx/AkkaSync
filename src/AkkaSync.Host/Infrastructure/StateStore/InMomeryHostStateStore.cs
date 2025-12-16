using System;
using AkkaSync.Host.Application.Dashboard;
using AkkaSync.Host.Domain.Entities;

namespace AkkaSync.Host.Infrastructure.StateStore;

public class InMomeryHostStateStore : IHostStateStore
{
  private HostSnapshot _snapshot = HostSnapshot.Empty;
  private readonly ReaderWriterLockSlim _lock = new ();
  public HostSnapshot GetSnapshot()
  {
    _lock.EnterReadLock();
    try
    {
      return _snapshot;
    }
    finally
    {
      _lock.ExitReadLock();
    }
  }

  public void Update(HostSnapshot snapshot)
  {
    _lock.EnterWriteLock();
    try
    {
      _snapshot = snapshot;
    }
    finally
    {
      _lock.ExitWriteLock();
    }
  }
}
