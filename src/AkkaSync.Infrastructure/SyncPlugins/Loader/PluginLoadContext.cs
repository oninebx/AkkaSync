
using System.Reflection;
using System.Runtime.Loader;

namespace AkkaSync.Infrastructure.SyncPlugins.PluginProviders
{
  public class PluginLoadContext : AssemblyLoadContext
  {
    private readonly string _pluginPath;
    public PluginLoadContext(string pluginPath): base(isCollectible: true)
    {
      _pluginPath = pluginPath;
    }
    protected override Assembly Load(AssemblyName assemblyName)
    {

      string candidate = Path.Combine(Path.GetDirectoryName(_pluginPath)!, assemblyName.Name + ".dll");
      if (File.Exists(candidate))
      {
        return LoadFromAssemblyPath(candidate);
      }
      return null!;
    }

    public Assembly LoadPlugin() => LoadFromAssemblyPath(_pluginPath);
  }
}
