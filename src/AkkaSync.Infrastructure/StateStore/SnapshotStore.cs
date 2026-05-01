using AkkaSync.Abstractions;
using System.Collections.ObjectModel;

namespace AkkaSync.Infrastructure.StateStore
{
  public sealed class SnapshotStore : ISnapshotStore
  {
    private readonly Dictionary<Type, Dictionary<string, ISnapshot>> _storage = [];
    public IReadOnlyDictionary<Type, IReadOnlyList<ISnapshot>> GetCurrent() => _storage.ToDictionary(
        pair => pair.Key,
        pair => (IReadOnlyList<ISnapshot>)pair.Value.Values.ToList()
    );

    public IReadOnlyDictionary<string, ISnapshot> GetCurrentByType(Type type)
    {
      if (_storage.TryGetValue(type, out var bucket))
      {
        return bucket.AsReadOnly();
      }
      return new ReadOnlyDictionary<string, ISnapshot>(new Dictionary<string, ISnapshot>());
    }

    public void Update(IEnumerable<ISnapshot> states)
    {
      if (states == null) return;

      foreach (var state in states)
      {
        InternalUpdate(state);
      }
    }

    private void InternalUpdate(ISnapshot state)
    {
      var type = state.GetType();

      if (!_storage.TryGetValue(type, out var bucket))
      {
        bucket = new Dictionary<string, ISnapshot>();
        _storage[type] = bucket;
      }

      bucket[state.Identifier] = state;
    }
  }
}
