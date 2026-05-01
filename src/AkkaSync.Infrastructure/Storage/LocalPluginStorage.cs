using System;
using System.Collections.Immutable;
using System.Text.Json;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Plugins.Models;
using AkkaSync.Infrastructure.Abstractions;
using AkkaSync.Infrastructure.SyncPlugins.Models;

namespace AkkaSync.Infrastructure.Storage;

public class LocalPluginStorage : IPluginStorage
{
  public string Key => "local";

  public string PluginFolder => _pluginDirectory;

  public string ShadowFolder => _shadowDirectory;

  private readonly string _pluginDirectory;
  private readonly string _shadowDirectory;
  private readonly JsonSerializerOptions options = new()
  {
    PropertyNameCaseInsensitive = true
  };

  public LocalPluginStorage(string pluginDirectory, string shadowDirectory)
  {
    _pluginDirectory = pluginDirectory;
    _shadowDirectory = shadowDirectory;
  }

  public async Task<string> SaveAsync(string pluginId, Stream content, CancellationToken cancellationToken = default)
  {
    var filePath = ResolvePluginPackageFile(pluginId);
    using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
    await content.CopyToAsync(fileStream, cancellationToken);
    return filePath;

  }

  //public Task EnsureAsync(IEnumerable<string> required)
  //{
  //  if (!Directory.Exists(_pluginsDirectory))
  //  {
  //    Directory.CreateDirectory(_pluginsDirectory);
  //  }

  //  var existingFiles = Directory.GetFiles(_pluginsDirectory, "AkkaSync.Plugins*.dll", SearchOption.TopDirectoryOnly)
  //    .Select(Path.GetFileName)
  //    .ToHashSet(StringComparer.OrdinalIgnoreCase);

  //  var missingFiles = required.Where(r => !existingFiles.Contains(r)).ToList();
  //  if (missingFiles.Count != 0)
  //  {
  //    throw new FileNotFoundException($"Missing plugin files: {string.Join(", ", missingFiles)}");
  //  }

  //  return Task.CompletedTask;
  //}

  //public IReadOnlySet<PluginPackageEntry> Diff(IReadOnlySet<PluginPackageRegistry> toCompare)
  //{
  // var localEntries = Directory.GetFiles(_pluginDirectory, "registry*.json")
  //      .Select(file => JsonSerializer.Deserialize<PluginPackageRegistry>(File.ReadAllText(file), options))
  //      .Where(r => r is not null)
  //      .SelectMany(r => r!.Plugins)
  //      .ToHashSet();

  //  return toCompare
  //      .SelectMany(r => r.Plugins)
  //      .Where(entry => !localEntries.Contains(entry))
  //      .ToHashSet();

  //}

  public async Task DeleteAsync(string pluginId, CancellationToken cancellationToken = default)
  {
    if (cancellationToken.IsCancellationRequested)
    {
      return;
    }
    if (string.IsNullOrWhiteSpace(pluginId))
    {
      return;
    }
    var shadowPath = ResolveShadowFolder(pluginId);
    if (!Directory.Exists(shadowPath))
    {
      return;
    }
    try
    {
      Directory.Delete(shadowPath, true);
      
      var pkgFile = ResolvePluginPackageFile(pluginId);
      if (File.Exists(pkgFile))
      {
        File.Delete(pkgFile);
      }
    }
    catch
    {
      throw;
    }
  }

  public async Task DeleteRangeAsync(IEnumerable<string> pluginIds, CancellationToken cancellationToken = default)
  {
    if (pluginIds == null)
    {
      return;
    }
    var exceptions = new List<Exception>();
    foreach (var pluginId in pluginIds)
    {
      if (cancellationToken.IsCancellationRequested)
      {
        break;
      }
      try
      {
        await DeleteAsync(pluginId, cancellationToken);
      }
      catch (Exception ex)
      {
        exceptions.Add(ex);
      }
    }
    if (exceptions.Count != 0) 
    {
      throw new AggregateException(exceptions);
    }
  }

  private string ResolveShadowFolder(string id) => Path.GetFullPath(Path.Combine(_shadowDirectory, id));
  private string ResolvePluginPackageFile(string id) => Path.GetFullPath(Path.Combine(_pluginDirectory, $"{id}.zip"));

  public IReadOnlyList<string> GetPluginFiles() => Directory.GetFiles(_shadowDirectory, "AkkaSync.Plugins*.dll", SearchOption.AllDirectories);

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
