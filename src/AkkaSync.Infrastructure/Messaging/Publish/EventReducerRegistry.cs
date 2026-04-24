using System;
using AkkaSync.Abstractions;
using AkkaSync.Infrastructure.Messaging.Publish;

namespace AkkaSync.Infrastructure.Messaging.Publish;

public sealed class EventReducerRegistry
{
  private readonly IReadOnlyDictionary<Type, Func<IStateSnashot, IProjectionEvent, IStateSnashot>> _reducers;

  internal EventReducerRegistry(IReadOnlyDictionary<Type, Func<IStateSnashot, IProjectionEvent, IStateSnashot>> reducers)
  {
    _reducers = new Dictionary<Type, Func<IStateSnashot, IProjectionEvent, IStateSnashot>>(reducers);
  }

  public bool TryReduce(IStateSnashot current, IProjectionEvent @event, out IStateSnashot next)
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
    private readonly Dictionary<Type, Func<IStateSnashot, IProjectionEvent, IStateSnashot>> _reducers = [];

    public EventReducerRegistryBuilder Add<TState>(
        Func<TState, IProjectionEvent, TState> reducer)
        where TState : class, IStateSnashot
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

