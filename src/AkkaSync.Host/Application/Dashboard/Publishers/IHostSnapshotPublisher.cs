using System;
using AkkaSync.Host.Domain.Entities;

namespace AkkaSync.Host.Application.Dashboard.Publishers;

public interface IHostSnapshotPublisher : IReadModelNotifier<HostSnapshot>
{
  Task PublishCurrentAsync(string connectionId);
}
