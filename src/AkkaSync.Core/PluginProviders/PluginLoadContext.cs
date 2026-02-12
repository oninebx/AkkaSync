using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Core.PluginProviders
{
  public class PluginLoadContext : AssemblyLoadContext
  {
    private readonly string _pluginPath;
    private readonly AssemblyDependencyResolver _resolver;
    public PluginLoadContext(string pluginPath): base(isCollectible: true)
    {
      _pluginPath = pluginPath;
      _resolver = new AssemblyDependencyResolver(pluginPath);
    }
    protected override Assembly Load(AssemblyName assemblyName)
    {
      
      var path = _resolver.ResolveAssemblyToPath(assemblyName);

      if (path is not null && File.Exists(path))
      {
        return LoadFromStream(path);
      }
      return null!;
    }

    public Assembly LoadPlugin() => LoadFromStream(_pluginPath);

    private Assembly LoadFromStream(string path)
    {
      using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      using var ms = new MemoryStream();
      fs.CopyTo(ms);
      ms.Position = 0;
      return LoadFromStream(ms);
    }
  }
}
