using Akka.Util;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Pipelines;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AkkaSync.Infrastructure.Storage;

public class LocalPipelineStorage : IPipelineStorage
{
  public string Key => "local";
  private readonly string _pipelineDirectory;
  private readonly ILogger _logger;
  public LocalPipelineStorage(string pipelineDirectory, ILogger<LocalPipelineStorage> logger)
  {
    _pipelineDirectory = pipelineDirectory;
    _logger = logger;
  }

  public async Task<Dictionary<string, PipelineSpec>> LoadPipelineSpecificationsAsync(CancellationToken cancellationToken = default) => await LoadSpecificationAsync<PipelineOptions, PipelineSpec>("pipeline*.json", x => x.Pipelines, cancellationToken);
  public async Task<Dictionary<string, ScheduleSpec>> LoadScheduleSpecificationsAsync(CancellationToken cancellationToken = default) => await LoadSpecificationAsync<ScheduleOptions, ScheduleSpec>("schedules*.json", x => x.Schedules, cancellationToken);

  private async Task<Dictionary<string, TSpec>> LoadSpecificationAsync<TSpecHolder, TSpec>(string filePattern, Func<TSpecHolder, IDictionary<string, TSpec>?> selector, CancellationToken cancellationToken = default)
  {
    var specs = new Dictionary<string, TSpec>();
    if (!Directory.Exists(_pipelineDirectory))
    {
      _logger.LogWarning("Directory not found: {Directory}", _pipelineDirectory);
      return specs;
    }
    var specFiles = Directory.GetFiles(_pipelineDirectory, filePattern, SearchOption.AllDirectories);

    if (specFiles.Length == 0)
    {
      _logger.LogWarning("No pipeline files found in directory: {Directory}", _pipelineDirectory);
      return specs;
    }

    var converterOptions = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
      Converters = { new NumberToStringConverter() }
    };

    foreach (var file in specFiles)
    {
      var json = await File.ReadAllTextAsync(file, cancellationToken);

      using var doc = JsonDocument.Parse(json);
      var syncOptions = doc.Deserialize<TSpecHolder>(converterOptions);

      if (syncOptions is null)
      {
        _logger.LogError("Failed to deserialize pipeline file: {File}", file);
        continue;
      }

      var dict = selector(syncOptions);
      if(dict is null)
      {
        _logger.LogWarning("No specifications found in pipeline file: {File}", file);
        continue;
      }
      foreach (var kvp in dict)
      {
        specs[kvp.Key] = kvp.Value;
      }
    }

    return specs;

  }
}

public sealed record SyncOptions(Dictionary<string, PipelineSpec> Pipelines, Dictionary<string, ScheduleSpec> Schedules);
public class NumberToStringConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetDouble().ToString(),
            JsonTokenType.String => reader.GetString(),
            _ => throw new JsonException()
        };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value);
    }
}

