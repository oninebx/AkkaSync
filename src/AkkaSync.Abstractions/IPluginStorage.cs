
namespace AkkaSync.Abstractions;

public interface IPluginStorage
{
  string Key { get; }
  string PluginFolder { get; }
  string ShadowFolder { get; }
  Task<string> SaveAsync(string pluginId, Stream content, CancellationToken cancellationToken = default);
  Task DeleteAsync(string pluginId, CancellationToken cancellationToken = default);
  Task DeleteRangeAsync(IEnumerable<string> pluginIds, CancellationToken cancellationToken = default);
  IReadOnlyList<string> GetPluginFiles();

  //Task EnsureAsync(IEnumerable<string> required);

  //IReadOnlySet<PluginPackageEntry> Diff(IReadOnlySet<PluginPackageRegistry> toCompare);
}
