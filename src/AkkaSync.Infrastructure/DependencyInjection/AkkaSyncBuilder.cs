using Akka.Actor;
using AkkaSync.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AkkaSync.Infrastructure.DependencyInjection
{
  public sealed class AkkaSyncBuilder
  {
    internal AkkaSyncOptions Options { get; } = new ();
    internal IServiceCollection Services { get; }
    public AkkaSyncBuilder(IServiceCollection services) 
    { 
      Services = services;
    }

    public AkkaSyncBuilder UsePlugins(string folder)
    {
      Options.PluginFolder = folder;
      return this;
    }
    public AkkaSyncBuilder AddActorHook<TActor>(string name) where TActor: ActorBase
    {
      Options.HookActors.Add(name, typeof(TActor));
      return this;
    }
    public AkkaSyncBuilder AddPlugins()
    {
      try
      {
        var pluginFolder = Options.PluginFolder ?? AppContext.BaseDirectory;
        var pluginFiles = Directory.GetFiles(pluginFolder, "AkkaSync.Plugins*.dll", SearchOption.TopDirectoryOnly);
        foreach (var file in pluginFiles)
        {
          try
          {
            var assembly = Assembly.LoadFrom(file);
            var pluginTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface &&
                  (typeof(IPluginProvider<ISyncSource>).IsAssignableFrom(t)
                  || typeof(IPluginProvider<ISyncTransformer>).IsAssignableFrom(t)
                  || typeof(IPluginProvider<ISyncSink>).IsAssignableFrom(t)
                  || typeof(IPluginProvider<IHistoryStore>).IsAssignableFrom(t)));

            foreach (var type in pluginTypes)
            {
              var interfaces = type.GetInterfaces()
                                  .Where(i => i.IsGenericType
                                  && i.GetGenericTypeDefinition() == typeof(IPluginProvider<>));
              foreach (var iface in interfaces)
              {
                Services.AddSingleton(iface, type);
                Console.WriteLine("Load plugin {0} successfully.", type.Name);
              }
            }
          }
          catch (ReflectionTypeLoadException rex)
          {
            foreach (var loaderException in rex.LoaderExceptions)
            {
              Console.WriteLine(loaderException?.GetType().FullName + ": " + loaderException?.Message);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Failed to load assembly: {0}", ex.Message);
      }
      return this;
    }
      
  }
}
