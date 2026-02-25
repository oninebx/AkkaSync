using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Host.Application.Dashboard;

public sealed class EventReducerRegistry
{
  private readonly IReadOnlyDictionary<Type, Func<IStoreValue, INotificationEvent, IStoreValue>> _reducers;

  internal EventReducerRegistry(IReadOnlyDictionary<Type, Func<IStoreValue, INotificationEvent, IStoreValue>> reducers)
  {
    _reducers = new Dictionary<Type, Func<IStoreValue, INotificationEvent, IStoreValue>>(reducers);
  }

  public bool TryReduce(IStoreValue current, INotificationEvent @event, out IStoreValue next)
  {
    if(_reducers.TryGetValue(current.GetType(), out var reducer))
    {
      next = reducer(current, @event);
      return true;
    }
    next = current;
    return false;
  }
}

public sealed class EventReducerRegistryBuilder
{
    private readonly Dictionary<Type, Func<IStoreValue, INotificationEvent, IStoreValue>> _reducers = [];

    public EventReducerRegistryBuilder Add<TState>(
        Func<TState, INotificationEvent, TState> reducer)
        where TState : class, IStoreValue
    {
        var stateType = typeof(TState);

        if (_reducers.ContainsKey(stateType))
            throw new InvalidOperationException(
                $"Reducer already registered for {stateType.Name}");

        _reducers[stateType] = (state, evt) =>
            reducer((TState)state, evt);

        return this;
    }

    public EventReducerRegistry Build()
        => new(_reducers);
}

