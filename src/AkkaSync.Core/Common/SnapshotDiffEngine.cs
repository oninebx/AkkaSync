using AkkaSync.Abstractions;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace AkkaSync.Core.Common
{
  public static class SnapshotDiffEngine
  {
    public static IEnumerable<R> GenerateDiff<T, R>(
      IReadOnlyList<T> currents, 
      IReadOnlyList<T> nexts,
      bool forceReplace = false) 
      where T : class, ISnapshot
      where R: class, IChangeSet
    {

      if (forceReplace)
      {
        yield return CreateInstance<R>(ChangeOperation.Replace, nexts.ToList());
        yield break;
      }

      var currentDict = currents.ToDictionary(c => c.Identifier);
      var nextIds = nexts.Select(n => n.Identifier).ToHashSet();

      var removedIds = currents
        .Where(c => !nextIds.Contains(c.Identifier))
        .Select(c => c.Identifier)
        .ToList();

      if (removedIds.Count > 0) 
      {
        yield return CreateInstance<R>(ChangeOperation.Remove, removedIds);
      }

      var upsertPayloads = new List<object>();
      foreach (var next in nexts)
      {
        if (!currentDict.TryGetValue(next.Identifier, out var current))
        {
          upsertPayloads.Add(next);
        }
        else if (!next.Equals(current))
        {
          var changes = GetPropertyDelta(current, next);

          if (changes != null)
          {
            var flatObj = new ExpandoObject() as IDictionary<string, object>;
            
            flatObj["identifier"] = next.Identifier;
            foreach (var pair in changes)
            {
              flatObj[pair.Key] = pair.Value;
            }
            upsertPayloads.Add(flatObj);
          }
        }
      }

      if (upsertPayloads.Count > 0)
      {
        yield return CreateInstance<R>(ChangeOperation.Upsert, upsertPayloads);
      }
    }

    private static readonly ConcurrentDictionary<Type, Func<ChangeOperation, object>> _constructorCache = new();

    private static R CreateInstance<R>(ChangeOperation op, object payload) where R : class, IChangeSet
    {
      var type = typeof(R);

      var factory = _constructorCache.GetOrAdd(type, t =>
      {
        var param = Expression.Parameter(typeof(ChangeOperation), "op");
        var ctor = t.GetConstructor(BindingFlags.Public | BindingFlags.Instance, [typeof(ChangeOperation)])
                   ?? throw new InvalidOperationException($"{t.Name} must contain a constructor that accepts a ChangeOperation.");

        var newExp = Expression.New(ctor, param);
        return Expression.Lambda<Func<ChangeOperation, object>>(newExp, param).Compile();
      });

      var instance = (R)factory(op);

      var prop = type.GetProperty(nameof(IChangeSet.Payload));
      if (prop != null && prop.CanWrite)
      {
        prop.SetValue(instance, payload);
      }

      return instance;
    }

    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache = new();

    private static IDictionary<string, object>? GetPropertyDelta<T>(T current, T next) where T : class
    {
      var type = typeof(T);
      var props = _propertyCache.GetOrAdd(type, t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                     .Where(p => p.Name != "Identifier" && p.Name != "Name")
                                                     .ToArray());

      var delta = new ExpandoObject() as IDictionary<string, object>;
      bool hasChanges = false;

      foreach (var prop in props)
      {
        var v1 = prop.GetValue(current);
        var v2 = prop.GetValue(next);
        if (!Equals(v1, v2))
        {
          delta[prop.Name] = v2!;
          hasChanges = true;
        }
      }
      return hasChanges ? delta : null;
    }
  }
}
