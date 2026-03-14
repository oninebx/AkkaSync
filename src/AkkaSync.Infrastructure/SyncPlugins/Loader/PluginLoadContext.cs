
using System.Reflection;
using System.Runtime.Loader;

namespace AkkaSync.Infrastructure.SyncPlugins.PluginProviders
{
  public class PluginLoadContext : AssemblyLoadContext
  {
    private readonly string _pluginPath;
    private readonly AssemblyDependencyResolver resolver;

    public PluginLoadContext(string pluginPath): base(isCollectible: true)
    {
      _pluginPath = pluginPath;
      resolver = new AssemblyDependencyResolver(pluginPath);
    }
    protected override Assembly Load(AssemblyName assemblyName)
    {
      var path = resolver.ResolveAssemblyToPath(assemblyName);
      if (path != null)
        return LoadFromAssemblyPath(path);

      return null!;
    }

    protected override nint LoadUnmanagedDll(string unmanagedDllName)
    {
      var path = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
      if (path != null)
        return LoadUnmanagedDllFromPath(path);

      return IntPtr.Zero;
    }

    public Assembly LoadPlugin() => LoadFromAssemblyPath(_pluginPath);
  }
}
