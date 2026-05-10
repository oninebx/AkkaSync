using AkkaSync.Abstractions;

namespace AkkaSync.Abstractions
{
  public interface ISnapshotStore
  {
    void Update(IEnumerable<ISnapshot> states);
    IReadOnlyDictionary<Type, IReadOnlyList<ISnapshot>> GetCurrent();
    IReadOnlyDictionary<string, ISnapshot> GetCurrentByType(Type type);
    void ResetByType(Type type);
    void RemoveByType(Type type, string id);
    void RemoveRangeByType(Type type, IEnumerable<string> ids);

    //IReadOnlyList<T> GetCurrentByType<T>() where T : TValue;
  }
}
