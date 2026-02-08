using AkkaSync.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure
{
  public record PluginLoadResult(Type InterfaceType, object ProviderInstance);
  public static class PluginLoader
  {
    private static readonly Type[] SupportedPluginInterfaces = new[]
    {
        typeof(IPluginProvider<ISyncSource>),
        typeof(IPluginProvider<ISyncTransformer>),
        typeof(IPluginProvider<ISyncSink>),
        typeof(IPluginProvider<IHistoryStore>)
    };

    public static IEnumerable<PluginLoadResult> LoadFromFile(string filePath, IServiceProvider serviceProvider)
    {
      var results = new List<PluginLoadResult>();

      try
      {
        var assembly = Assembly.LoadFrom(filePath);

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
              results.Add(new PluginLoadResult(iface, instance));
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

      return results;
    }
  }
}
