using System;

namespace AkkaSync.Publish;

using System.Text.Json;

public record PublishConfig(
  string ReleaseVersion,
  string AkkaSyncVersion,
  string Demo,
  string PluginFolder,
  string ExamplesRoot
);
public static class PublishHelper
{
  public static PublishConfig LoadConfig()
  {
    var configPath = Path.Combine(AppContext.BaseDirectory, "publish.json");

    if (!File.Exists(configPath))
      throw new FileNotFoundException("publish.json not found", configPath);

    var configContent = File.ReadAllText(configPath);

    var version = JsonSerializer.Deserialize<PublishConfig>(configContent)
        ?? throw new Exception("Failed to parse publish.json");
    return version;
  }

  public static void CopyDirectory(string source, string target)
  {
    if (!Directory.Exists(target))
    {
      Directory.CreateDirectory(target);
    }

    foreach (var dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
    {
      Directory.CreateDirectory(dir.Replace(source, target));
    }

    foreach (var file in Directory.GetFiles(source, "*.*", SearchOption.AllDirectories))
    {
      File.Copy(file, file.Replace(source, target), overwrite: true);
    }
  }
}

