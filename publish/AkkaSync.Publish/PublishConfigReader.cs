using System;

namespace AkkaSync.Publish;

using System.Text.Json;

public record PublishConfig(
  string ReleaseVersion,
  string AkkaSyncVersion,
  string Demo,
  string ExamplesRoot
);
public static class PublishConfigReader
{
  public static PublishConfig Load()
  {
    var configPath = Path.Combine(AppContext.BaseDirectory, "publish.json");

    if (!File.Exists(configPath))
      throw new FileNotFoundException("publish.json not found", configPath);

    var configContent = File.ReadAllText(configPath);

    var version = JsonSerializer.Deserialize<PublishConfig>(configContent)
        ?? throw new Exception("Failed to parse publish.json");
    return version;
  }
}

