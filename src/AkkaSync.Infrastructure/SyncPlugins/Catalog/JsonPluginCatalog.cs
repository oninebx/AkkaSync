using AkkaSync.Infrastructure.SyncPlugins.Models;
using System.Text.Json;

namespace AkkaSync.Infrastructure.SyncPlugins.Catalog
{
  public class JsonPluginCatalog : IPluginCatalog
  {
    private readonly string _filePath;
    private bool _isInitialized = false;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true, PropertyNameCaseInsensitive = true };
    private List<PluginCatalogEntry> _catalog = [];

    public JsonPluginCatalog(string filePath)
    {
      _filePath = filePath;
    }
    public async Task AddAsync(PluginCatalogEntry entry)
    {
      await _lock.WaitAsync();
      try
      {
        await EnsureInitializedAsync();
        _catalog.Add(entry);
        await SaveAsync();
      }
      finally
      {
        _lock.Release();
      }
      
    }

    public async Task<IReadOnlyList<PluginCatalogEntry>> GetAllAsync(Func<PluginCatalogEntry, bool>? predicate = null)
    {
      await EnsureInitializedAsync();
      var query = _catalog.AsEnumerable();
      if (predicate != null)
      {
        query = query.Where(predicate);
      }
      return query.ToList().AsReadOnly();
    }

    public async Task RemoveAsync(string id, string version)
    {
      await _lock.WaitAsync();
      try
      {
        await EnsureInitializedAsync();
        var item = _catalog.FirstOrDefault(p => p.Id == id && p.Version == version);
        if (item != null)
        {
          _catalog.Remove(item);
          await SaveAsync();
        }
      }
      finally
      {
        _lock.Release();
      }
      
    }

    public async Task UpdateAsync(PluginCatalogEntry entry)
    {
      await _lock.WaitAsync();
      try
      {
        await EnsureInitializedAsync();
        var index = _catalog.FindIndex(p => p.Id == entry.Id && p.Version == entry.Version);
        if (index != -1)
        {
          _catalog[index] = entry;
          await SaveAsync();
        }
      }
      finally
      {
        _lock.Release();
      }
    }

    private async Task SaveAsync()
    {
      var json = JsonSerializer.Serialize(_catalog, new JsonSerializerOptions
      {
        WriteIndented = true
      });

      await File.WriteAllTextAsync(_filePath, json);
      
    }

    private async Task EnsureInitializedAsync()
    {
      if (_isInitialized)
      {
        return;
      }

      if (File.Exists(_filePath))
      {
        using var stream = File.OpenRead(_filePath);
        _catalog = await JsonSerializer.DeserializeAsync<List<PluginCatalogEntry>>(stream, _jsonOptions) ?? [];
      }
      _isInitialized = true;
    }
  }
}
