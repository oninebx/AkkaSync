using AkkaSync.Infrastructure.SyncPlugins.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.SyncPlugins.Catalog
{
  public class JsonPluginCatalog : IPluginCatalog
  {
    private readonly string _filePath;
    private readonly List<PluginCatalogEntry> _catalog;

    public JsonPluginCatalog(string filePath)
    {
      _filePath = filePath;
      var json = File.Exists(filePath) ? File.ReadAllText(filePath) : null;
      _catalog = !string.IsNullOrWhiteSpace(json)
        ? JsonSerializer.Deserialize<List<PluginCatalogEntry>>(json)!
        : [];
    }
    public async Task AddAsync(PluginCatalogEntry entry)
    {
      _catalog.Add(entry);
      await SaveAsync();
    }

    public Task<IReadOnlyList<PluginCatalogEntry>> GetAllAsync()
    {
      throw new NotImplementedException();
    }

    public Task RemoveAsync(string id, string version)
    {
      throw new NotImplementedException();
    }

    public Task UpdateAsync(PluginCatalogEntry entry)
    {
      throw new NotImplementedException();
    }

    private async Task SaveAsync()
    {
      var json = JsonSerializer.Serialize(_catalog, new JsonSerializerOptions
      {
        WriteIndented = true
      });

      await File.WriteAllTextAsync(_filePath, json);
    }
  }
}
