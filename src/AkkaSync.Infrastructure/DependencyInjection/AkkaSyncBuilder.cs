using Akka.Actor;
using AkkaSync.Abstractions;
using AkkaSync.Core.PluginProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using AkkaSync.Infrastructure.PipelineStorages;
using AkkaSync.Infrastructure.PluginStorages;

namespace AkkaSync.Infrastructure.DependencyInjection
{
  public sealed class AkkaSyncBuilder
  {
    internal AkkaSyncOptions Options { get; } = new();
    private IServiceCollection _services { get; }
    private StorageOptions _pipelineStorageOptions { get; }
    private StorageOptions _pluginStorageOptions { get; }
    public AkkaSyncBuilder(IServiceCollection services, IConfiguration configuration)
    {
      _services = services;
      _pluginStorageOptions = configuration.GetSection("PluginStorage").Get<StorageOptions>() ?? new StorageOptions("Local", "plugins");
      _pipelineStorageOptions = configuration.GetSection("PipelineStorage").Get<StorageOptions>() ?? new StorageOptions("Local", "pipelines");
    }

    public AkkaSyncBuilder AddPipelines()
    {
      _services.AddSingleton<IPipelineStorage>(sp =>
      {
        var options = _pipelineStorageOptions;

        return options.Type switch
        {
          "Local" => ActivatorUtilities.CreateInstance<LocalPipelineStorage>(sp, Path.Combine(AppContext.BaseDirectory, options.Uri)),
          _ => throw new NotSupportedException($"Pipeline storage type {options.Type} is not supported.")
        };
      });
      return this;
    }

    public AkkaSyncBuilder AddPlugins()
    {
      // build plugin storage
      IPluginStorage pluginStorage = _pluginStorageOptions.Type switch
      {
        "Local" => new LocalPluginStorage(_pluginStorageOptions.Uri),
        _ => throw new NotSupportedException($"Plugin storage type {_pluginStorageOptions.Type} is not supported.")
      };
      _services.AddSingleton(pluginStorage);

      var shadowFolder = Path.Combine(AppContext.BaseDirectory, "shadow"); //  local plugin shadow copy folder for loading to avoid file lock
      var pluginFolder = Path.Combine(AppContext.BaseDirectory, "plugins"); // local plugin cache folder
      Options.PluginFolder = pluginFolder;
      Options.ShadowFolder = shadowFolder;
      PrepareShadowFolder(pluginFolder, shadowFolder);
      LoadPlugins(shadowFolder);
      return this;
    }
    public AkkaSyncBuilder AddActorHook<TActor>(string name) where TActor : ActorBase
    {
      Options.HookActors.Add(name, typeof(TActor));
      return this;
    }

    private static void PrepareShadowFolder(string pluginFolder, string shadowFolder)
    {
      if (Directory.Exists(shadowFolder))
      {
        Directory.Delete(shadowFolder, true);
      }
      Directory.CreateDirectory(shadowFolder);
      var pluginFiles = Directory.GetFiles(pluginFolder, "AkkaSync.Plugins*.dll", SearchOption.TopDirectoryOnly);
      foreach (var pluginFile in pluginFiles)
      {
        var destFile = Path.Combine(shadowFolder, Path.GetFileName(pluginFile));
        File.Copy(pluginFile, destFile, true);
      }
    }

    private AkkaSyncBuilder LoadPlugins(string shadowFolder)
    {
      try
      {
        var pluginFiles = Directory.GetFiles(shadowFolder, "AkkaSync.Plugins*.dll", SearchOption.TopDirectoryOnly);
        foreach (var file in pluginFiles)
        {
          try
          {
            var (context, types) = PluginLoader.LoadPluginTypes(file);

            foreach (var type in types)
            {
              var interfaces = type.GetInterfaces()
                                  .Where(i => i.IsGenericType
                                  && i.GetGenericTypeDefinition() == typeof(IPluginProvider<>));
              foreach (var iface in interfaces)
              {
                _services.AddSingleton(iface, type);
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
