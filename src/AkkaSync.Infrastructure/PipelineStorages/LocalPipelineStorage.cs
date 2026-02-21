using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace AkkaSync.Infrastructure.PipelineStorages;

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

  public async Task<(PipelineOptions, ScheduleOptions)> LoadSpecificationsAsync(CancellationToken cancellationToken = default)
  {
    if (!Directory.Exists(_pipelineDirectory))
    {
      throw new DirectoryNotFoundException($"Directory not found: {_pipelineDirectory}");
    }
    var pipelineFiles = Directory.GetFiles(_pipelineDirectory, "pipeline_*.json", SearchOption.AllDirectories);
    
 
    if (pipelineFiles.Length == 0)
    {
      throw new FileNotFoundException($"No pipeline files found in directory: {_pipelineDirectory}");
    }
    var selectedFiles = pipelineFiles.ToList();
    var pipelines = new Dictionary<string, PipelineSpec>();
    var schedules = new Dictionary<string, ScheduleSpec>();
    var converterOptions = new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true,
      Converters = { new NumberToStringConverter(),  }
    };
    foreach (var file in selectedFiles)
    {
      var json = await File.ReadAllTextAsync(file, cancellationToken);
      using var doc = JsonDocument.Parse(json);
      var syncOptions = doc.RootElement.GetProperty("AkkaSync").Deserialize<SyncOptions>(converterOptions);
      if (syncOptions is null)
      {
        _logger.LogError("Failed to deserialize pipeline file: {0}", file);
        continue;
      }
      var filePipelines = syncOptions.Pipelines;
      if (filePipelines is not null)
      {
        foreach (var kvp in filePipelines)
        {
          pipelines[kvp.Key] = kvp.Value;
        }
      }

      var fileSchedules = syncOptions.Schedules;
      if (fileSchedules is not null)
      {
        foreach (var kvp in fileSchedules)
        {
          schedules[kvp.Key] = kvp.Value;
        }
      }
      
    }
    return (new PipelineOptions { Pipelines = pipelines }, new ScheduleOptions { Schedules = schedules });
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
