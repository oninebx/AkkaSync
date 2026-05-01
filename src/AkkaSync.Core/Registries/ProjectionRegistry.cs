using AkkaSync.Abstractions;

namespace AkkaSync.Core.Registries
{
  public sealed class ProjectionRegistry
  {
    private readonly Dictionary<Type, Func<IReadOnlyList<ISnapshot>, IReadOnlyList<ISnapshot>, IReadOnlyList<IChangeSet>>> _projections;

    internal ProjectionRegistry(IReadOnlyDictionary<Type, Func<IReadOnlyList<ISnapshot>, IReadOnlyList<ISnapshot>, IReadOnlyList<IChangeSet>>> projections) 
    {
      _projections = new Dictionary<Type, Func<IReadOnlyList<ISnapshot>, IReadOnlyList<ISnapshot>, IReadOnlyList<IChangeSet>>>(projections);
    }

    public bool TryProjection(Type type, IReadOnlyList<ISnapshot> currents, IReadOnlyList<ISnapshot> nexts, out IReadOnlyList<IChangeSet> changes)
    {
      if (_projections.TryGetValue(type, out var projection))
      {
        changes =  projection(currents, nexts) ?? [];
        return changes.Count != 0;
      }
      changes = [];
      return false;
    }
  }

  public sealed class ProjectionRegistryBuilder
  {
    private readonly Dictionary<Type, Func<IReadOnlyList<ISnapshot>, IReadOnlyList<ISnapshot>, IReadOnlyList<IChangeSet>>> _projections = [];

    public ProjectionRegistryBuilder Add<TState>(
        Func<IReadOnlyList<TState>, IReadOnlyList<TState>, IReadOnlyList<IChangeSet>> projection)
        where TState : class, ISnapshot
    {
      var stateType = typeof(TState);

      if (_projections.ContainsKey(stateType))
        throw new InvalidOperationException(
            $"Projection already registered for {stateType.Name}");

      _projections[stateType] = (currents, nexts) => 
      {
        var c = currents.Cast<TState>().ToList();
        var n = nexts.Cast<TState>().ToList();
        return projection(c, n);
      };

      return this;
    }

    public ProjectionRegistry Build()
        => new(_projections);
  }
}
