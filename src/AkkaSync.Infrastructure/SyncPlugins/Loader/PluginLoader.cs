using AkkaSync.Abstractions;
using AkkaSync.Infrastructure.SyncPlugins.PluginProviders;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AkkaSync.Infrastructure.SyncPlugins.Loader
{
  public record PluginLoadResult(Type InterfaceType, object ProviderInstance, PluginLoadContext LoadContext)
  {

  };
  public static class PluginLoader
  {
    private static readonly Type[] SupportedPluginInterfaces =
    [
        typeof(IPluginProvider<ISyncSource>),
        typeof(IPluginProvider<ISyncTransformer>),
        typeof(IPluginProvider<ISyncSink>),
        typeof(IPluginProvider<IHistoryStore>)
    ];

    
    public static async Task EnsurePluginZipFileCreated(string path)
    {
      try
      {
        await WaitForFileStableAsync(path);
        await WaitForFileUnlockAsync(path);
      }
      catch (Exception ex)
      {
        Console.WriteLine("Create plugin zip file failed in {0}: {1}", path, ex);
      }
    }

    public static (IEnumerable<PluginLoadResult> LoadResult, PluginLoadContext? LoadContext) LoadFromFile(string filePath, IServiceProvider serviceProvider)
    {
      var results = new List<PluginLoadResult>();
      PluginLoadContext context = null!;

      try
      {
        context = new PluginLoadContext(filePath);
        var assembly = context.LoadPlugin();

        var pluginTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface &&
                        SupportedPluginInterfaces.Any(iface => iface.IsAssignableFrom(t)));



        foreach (var type in pluginTypes)
        {
          var interfaces = type.GetInterfaces()
              .Where(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == typeof(IPluginProvider<>));

          foreach (var iface in interfaces)
          {
            var instance = ActivatorUtilities.CreateInstance(serviceProvider, type);
            if (instance != null)
            {
              results.Add(new PluginLoadResult(iface, instance, context));
              Console.WriteLine("Loaded plugin {0} implementing {1}", type.Name, iface.Name);
            }
          }
        }
      }
      catch (Exception ex)
      {
        if(ex is ReflectionTypeLoadException rex)
        {
          foreach (var loaderEx in rex.LoaderExceptions)
          {
            Console.WriteLine(loaderEx?.Message);
          }
        }
        
        context.Unload();
        context = null!;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        results.Clear();
      }

      return (results, context);
    }

    private static async Task WaitForFileStableAsync(string path)
    {
      long lastSize = -1;

      while (true)
      {
        if (!File.Exists(path))
        {
          await Task.Delay(500);
          continue;
        }

        long currentSize = new FileInfo(path).Length;

        if (currentSize == lastSize && currentSize > 0)
          break;

        lastSize = currentSize;
        await Task.Delay(1000);
      }
    }

    private static async Task WaitForFileUnlockAsync(string path)
    {
      while (true)
      {
        try
        {
          using var stream = new FileStream(
              path,
              FileMode.Open,
              FileAccess.Read,
              FileShare.None);

          break;
        }
        catch
        {
          await Task.Delay(500);
        }
      }
    }
  }
}
