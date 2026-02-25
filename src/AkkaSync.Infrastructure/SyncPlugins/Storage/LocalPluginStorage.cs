using System;
using AkkaSync.Abstractions;

namespace AkkaSync.Infrastructure.SyncPlugins.Storage;

public class LocalPluginStorage : IPluginStorage
{
  public string Key => "local";
  private readonly string _pluginsDirectory;

  public LocalPluginStorage(string pluginsDirectory)
  {
    _pluginsDirectory = pluginsDirectory;
  }

  public async Task<string> SaveAsync(string fileName, Stream content, CancellationToken cancellationToken = default)
  {
    var filePath = Path.Combine(_pluginsDirectory, fileName);
    using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
    await content.CopyToAsync(fileStream, cancellationToken);
    return filePath;
  }

  public Task EnsureAsync(IEnumerable<string> required)
  {
    if (!Directory.Exists(_pluginsDirectory))
    {
      Directory.CreateDirectory(_pluginsDirectory);
    }

    var existingFiles = Directory.GetFiles(_pluginsDirectory, "AkkaSync.Plugins*.dll", SearchOption.TopDirectoryOnly)
      .Select(Path.GetFileName)
      .ToHashSet(StringComparer.OrdinalIgnoreCase);

    var missingFiles = required.Where(r => !existingFiles.Contains(r)).ToList();
    if (missingFiles.Count != 0)
    {
      throw new FileNotFoundException($"Missing plugin files: {string.Join(", ", missingFiles)}");
    }

    return Task.CompletedTask;
  }

  // public Task<string[]> ListPluginsAsync(CancellationToken cancellationToken = default)
  // {
  //   if (!Directory.Exists(_pluginsDirectory))
  //   {
  //     return Task.FromResult(Array.Empty<string>());
  //   }

  //   var pluginFiles = Directory.GetFiles(_pluginsDirectory, "AkkaSync.Plugins*.dll", SearchOption.TopDirectoryOnly);
  //   return Task.FromResult(pluginFiles);
  // }
}
