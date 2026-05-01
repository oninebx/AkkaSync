using System;
using AkkaSync.Abstractions;
using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Infrastructure.Messaging.Publish;

public sealed class EventReducerRegistry
{
  private readonly IReadOnlyDictionary<Type, Func<ISnapshot, IProjectionEvent, ISnapshot>> _reducers;

  internal EventReducerRegistry(IReadOnlyDictionary<Type, Func<ISnapshot, IProjectionEvent, ISnapshot>> reducers)
  {
    _reducers = new Dictionary<Type, Func<ISnapshot, IProjectionEvent, ISnapshot>>(reducers);
  }

  public bool TryReduce(ISnapshot current, IProjectionEvent @event, out ISnapshot next)
  {
    if(_reducers.TryGetValue(current.GetType(), out var reducer))
    {
      next = reducer(current, @event);
      return !Equals(current, next);
    }
    next = current;
    return false;
  }
}

public sealed class EventReducerRegistryBuilder
{
    private readonly Dictionary<Type, Func<ISnapshot, IProjectionEvent, ISnapshot>> _reducers = [];

    public EventReducerRegistryBuilder Add<TState>(
        Func<TState, IProjectionEvent, TState> reducer)
        where TState : class, ISnapshot
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

