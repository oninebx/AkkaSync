using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

namespace AkkaSync.Abstractions.Models;

public record PipelineSpec
{
  public required string Name { get; init; }
  public bool AutoStart { get; init; } = true;
  public required PluginSpec SourceProvider { get; init; }
  public required PluginSpec TransformerProvider { get; init; }
  public required PluginSpec SinkProvider { get; init; }
  public required PluginSpec HistoryStoreProvider { get; init; }
}

public record PluginSpec
{
  public required string Type { get; init; }
  public IReadOnlyDictionary<string, string> Parameters { get; init; } = default!;
}

public static class ParametersExtensions
{

  public static T Get<T>(
        this IReadOnlyDictionary<string, string> parameters,
        string key,
        T defaultValue = default!
    )
  {
    if (!parameters.TryGetValue(key, out var raw))
      return defaultValue;

    if (string.IsNullOrWhiteSpace(raw))
      return defaultValue;

    var targetType = typeof(T);
    try
    {
      // string
      if (targetType == typeof(string))
        return (T)(object)raw;

      // Nullable<T>
      var underlying = Nullable.GetUnderlyingType(targetType);
      if (underlying != null)
      {
        return (T)ConvertScalar(raw, underlying)!;
      }
      // enum / primitive / value type
      if (IsScalar(targetType))
        return (T)ConvertScalar(raw, targetType)!;

      // object
      return JsonSerializer.Deserialize<T>(raw)!;
    }
    catch
    {
      return defaultValue;
    }
  }

  private static bool IsScalar(Type type)
  {
    return
        type.IsEnum ||
        type == typeof(Guid) ||
        type == typeof(DateTime) ||
        type == typeof(DateTimeOffset) ||
        type == typeof(TimeSpan) ||
        TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string));
  }

  private static object? ConvertScalar(string raw, Type targetType)
  {
    if (targetType.IsEnum)
      return Enum.Parse(targetType, raw, ignoreCase: true);

    if (targetType == typeof(Guid))
      return Guid.Parse(raw);

    if (targetType == typeof(DateTime))
      return DateTime.Parse(raw, CultureInfo.InvariantCulture);

    if (targetType == typeof(DateTimeOffset))
      return DateTimeOffset.Parse(raw, CultureInfo.InvariantCulture);

    if (targetType == typeof(TimeSpan))
      return TimeSpan.Parse(raw, CultureInfo.InvariantCulture);
    var converter = TypeDescriptor.GetConverter(targetType);
    return converter.ConvertFromInvariantString(raw);
  }

}