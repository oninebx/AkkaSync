using System;

namespace AkkaSync.Plugins.Transformer.DbTable;

using System;
using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static T? GetValueOrDefault<T>(this IReadOnlyDictionary<string, object?> row, string key, object? defaultValue = null)
    {
        if (row == null) throw new ArgumentNullException(nameof(row));

        if (!row.TryGetValue(key, out var obj) || obj == null)
            obj = defaultValue;

        if (obj == null)
            return default(T);

        try
        {
            Type targetType = typeof(T);
            Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (underlyingType == typeof(bool))
            {
                if (obj is bool b) return (T)(object)b;
                var str = obj.ToString();
                if (str == "0") return (T)(object)false;
                if (str == "1") return (T)(object)true;
            }

            return (T)Convert.ChangeType(obj, underlyingType);
        }
        catch
        {
            return default(T);
        }
    }
}

