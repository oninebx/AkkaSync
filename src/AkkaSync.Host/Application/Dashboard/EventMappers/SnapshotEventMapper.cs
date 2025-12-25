using System;
using AkkaSync.Abstractions;
using AkkaSync.Host.Domain.Dashboard.Services;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Application.Dashboard.EventMappers;

public static class SnapshotEventMapper
{
  public static HostSnapshot TryMap(HostSnapshot snapshot, ISyncEvent @event) => HostStateReducer.Reduce(snapshot, @event);
}