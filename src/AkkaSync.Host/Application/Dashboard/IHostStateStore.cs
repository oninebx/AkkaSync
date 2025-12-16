using System;
using AkkaSync.Host.Domain.Entities;

namespace AkkaSync.Host.Application.Dashboard;

public interface IHostStateStore
{
  HostSnapshot GetSnapshot();

  void Update(HostSnapshot snapshot);
}
