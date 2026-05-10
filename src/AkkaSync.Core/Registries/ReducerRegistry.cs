using AkkaSync.Abstractions;

namespace AkkaSync.Core.Registries
{
  public sealed class ReducerRegistry
  {
    private readonly IReadOnlyDictionary<Type, Func<ISnapshot?, ISnapshotEvent, string, ISnapshot>> _reducers;

    internal ReducerRegistry(IReadOnlyDictionary<Type, Func<ISnapshot?, ISnapshotEvent, string, ISnapshot>> reducers)
    {
      _reducers = new Dictionary<Type, Func<ISnapshot?, ISnapshotEvent, string, ISnapshot>>(reducers);
    }

    public bool TryReduce(Type targetType, string id, ISnapshot? current, ISnapshotEvent @event, out ISnapshot? next)
    {
      if (_reducers.TryGetValue(targetType, out var reducer))
      {
        next = reducer(current, @event, id);

        return current == null ? next != null : !ReferenceEquals(current, next);
      }

      next = current!;
      return false;
    }
  }

  public sealed class ReducerRegistryBuilder
  {
    private readonly Dictionary<Type, Func<ISnapshot?, ISnapshotEvent, string, ISnapshot>> _reducers = [];

    public ReducerRegistryBuilder AddReducer<TState>(
        Func<TState?, ISnapshotEvent, string, TState> reducer)
        where TState : class, ISnapshot
    {
      var stateType = typeof(TState);

      if (_reducers.ContainsKey(stateType))
        throw new InvalidOperationException(
            $"Reducer already registered for {stateType.Name}");

      _reducers[stateType] = (state, evt, id) =>
          reducer((TState?)state, evt, id);

      return this;
    }

    public ReducerRegistry Build()
        => new(_reducers);
  }
}
