using AkkaSync.Abstractions;

namespace AkkaSync.Core.Projection
{
  public sealed class ProjectionARegistry
  {
    private readonly IReadOnlyDictionary<Type, List<Func<ISnapshot, ISnapshot, IChangeSet>>> _projectionsMap;

    public ProjectionARegistry(IReadOnlyDictionary<Type, List<Func<ISnapshot, ISnapshot, IChangeSet>>> projectionsMap) 
    {
      _projectionsMap = projectionsMap;
    }

    public IReadOnlyList<IChangeSet> Project(ISnapshot previous, ISnapshot current)
    {
      if (previous is null || current is null)
      {
        throw new ArgumentNullException($"{nameof(previous)} and {nameof(current)} cannot be null.");
      }

      if (previous.GetType() != current.GetType())
      {
        throw new InvalidOperationException("State type mismatch");
      }

      if (!_projectionsMap.TryGetValue(previous.GetType(), out var projections))
      {
        return [];
      }

      var result = new List<IChangeSet>();

      foreach (var projection in projections)
      {
        var change = projection(previous, current);
        if (change != null)
        {
          result.Add(change);
        }
      }

      return result;
    }
  }

  public sealed class ProjectionARegistryBuilder
  {
    private readonly Dictionary<Type, List<Func<ISnapshot, ISnapshot, IChangeSet>>> _projections = [];

    public ProjectionARegistryBuilder Add<TState>(
        Func<TState, TState, IChangeSet> projection)
        where TState : ISnapshot
    {
      var stateType = typeof(TState);

      if (!_projections.TryGetValue(stateType, out var list))
      {
        list = [];
        _projections[stateType] = list;
      }

      list.Add((previous, current) =>  projection((TState)previous, (TState)current));

      return this;
    }


    public ProjectionARegistry Build()
        => new(_projections);
  }
}
