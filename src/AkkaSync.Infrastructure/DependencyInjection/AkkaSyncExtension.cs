using System;
using Akka.Actor;
using AkkaSync.Abstractions;
using AkkaSync.Core.Common;
using AkkaSync.Core.PluginProviders;
using Akka.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using AkkaSync.Core.Actors;
using System.Reflection;

namespace AkkaSync.Infrastructure.DependencyInjection;

public static class AkkaSyncExtension
{
  public static IServiceCollection AddAkkaSync(this IServiceCollection services, Action<ActorSystem, DependencyResolver>? config = null)
  {
    services.AddSingleton<ISyncGenerator, SyncGenerator>();
    services.AddSingleton<IPluginProviderRegistry<ISyncSource>, PluginProviderRegistry<ISyncSource>>();
    services.AddSingleton<IPluginProviderRegistry<ISyncTransformer>, PluginProviderRegistry<ISyncTransformer>>();
    services.AddSingleton<IPluginProviderRegistry<ISyncSink>, PluginProviderRegistry<ISyncSink>>();
    services.AddSingleton<IPluginProviderRegistry<IHistoryStore>, PluginProviderRegistry<IHistoryStore>>();

    services.AddSingleton(provider =>
    {
      var bootstrap = BootstrapSetup.Create();
      var di = DependencyResolverSetup.Create(provider);
      var system = ActorSystem.Create("AkkaSyncSystem", bootstrap.And(di));
      var resolver = DependencyResolver.For(system);
      system.ActorOf(resolver.Props<PipelineManagerActor>(), "pipeline-manager");
      config?.Invoke(system, resolver);
      return system;
    });

    
    return services;
  }

  public static IServiceCollection AddAkkaSyncPlugins(this IServiceCollection services, string pluginFolder)
  {
    try
    {
    pluginFolder ??= AppContext.BaseDirectory;
    var pluginFiles = Directory.GetFiles(pluginFolder, "AkkaSync.Plugins*.dll", SearchOption.TopDirectoryOnly);
    foreach(var file in pluginFiles)
      {
        try
        {
          var assembly = Assembly.LoadFrom(file);
          var pluginTypes = assembly.GetTypes()
              .Where(t => !t.IsAbstract && !t.IsInterface &&
                (typeof(IPluginProvider<ISyncSource>).IsAssignableFrom(t)
                || typeof(IPluginProvider<ISyncTransformer>).IsAssignableFrom(t)
                || typeof(IPluginProvider<ISyncSink>).IsAssignableFrom(t)));
            
          foreach(var type in pluginTypes)
          {
            var interfaces = type.GetInterfaces()
                                .Where(i => i.IsGenericType 
                                && i.GetGenericTypeDefinition() == typeof(IPluginProvider<>));
            foreach(var iface in interfaces)
            {
              services.AddSingleton(iface, type);
              Console.WriteLine("Load plugin {0} successfully.", type.Name);
            }
          }
        }
        catch(ReflectionTypeLoadException rex)
        {
          foreach (var loaderException in rex.LoaderExceptions)
          {
              Console.WriteLine(loaderException?.GetType().FullName + ": " + loaderException?.Message);
          }
        }
      }
    }
    catch(Exception ex)
    {
      Console.WriteLine("Failed to load assembly: {0}", ex.Message);
    }
    return services;
  }
}
