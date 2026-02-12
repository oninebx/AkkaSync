using AkkaSync.Abstractions;
using AkkaSync.Core.PluginProviders;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure
{
  public record PluginLoadResult(Type InterfaceType, object ProviderInstance, PluginLoadContext LoadContext);
  public static class PluginLoader
  {
    private static readonly Type[] SupportedPluginInterfaces = new[]
    {
        typeof(IPluginProvider<ISyncSource>),
        typeof(IPluginProvider<ISyncTransformer>),
        typeof(IPluginProvider<ISyncSink>),
        typeof(IPluginProvider<IHistoryStore>)
    };

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
      catch (ReflectionTypeLoadException rex)
      {
        foreach (var loaderEx in rex.LoaderExceptions)
        {
          Console.WriteLine(loaderEx?.Message);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Failed to load plugin from {filePath}: {ex.Message}");
      }

      return (results, context);
    }

    //public static void UnloadForFile(string filePath)
    //{
    //  if (_pluginContexts.TryGetValue(filePath, out var context))
    //  {
    //    context.Unload();

    //    GC.Collect();
    //    GC.WaitForPendingFinalizers();
    //    GC.Collect();

    //    _pluginContexts.Remove(filePath);
    //    Console.WriteLine($"Unloaded plugin from {filePath}");
    //  }
    //}
  }
}
