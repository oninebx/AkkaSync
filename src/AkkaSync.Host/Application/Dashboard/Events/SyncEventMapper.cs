using System;
using AkkaSync.Abstractions;
using AkkaSync.Host.Domain.Dashboard.Services;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Application.Dashboard.Events;

public static class SyncEventMapper
{
  private static readonly Func<IStoreValue, ISyncEvent, IStoreValue> HostReducer =
    (state, evt) => HostStateReducer.Reduce((HostSnapshot)state, evt);
  public static IReadOnlyList<Func<IStoreValue, ISyncEvent, IStoreValue>> Reducers = [
    HostReducer
  ];
}
