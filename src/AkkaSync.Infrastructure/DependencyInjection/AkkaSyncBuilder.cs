using Akka.Actor;
using AkkaSync.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using AkkaSync.Infrastructure.PipelineStorages;
using AkkaSync.Infrastructure.SyncPlugins.Storage;
using AkkaSync.Infrastructure.SyncPlugins.Loader;
using System.IO.Compression;
using AkkaSync.Infrastructure.SyncPlugins.PackageManager;
using AkkaSync.Infrastructure.Options;

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

      _services.Configure<PluginOptions>(configuration.GetSection("Plugins"));
      _pluginStorageOptions = configuration.GetSection("Plugins:Storage").Get<StorageOptions>() ?? new StorageOptions();
      _pipelineStorageOptions = configuration.GetSection("PipelineStorage").Get<StorageOptions>() ?? new StorageOptions();
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

      // build plugin package manager
      _services.AddHttpClient<IPluginHttpClient, PluginHttpClient>(client =>
      {
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("AkkaSync-PluginClient/1.0");
      });

      _services.AddSingleton<IPluginPackageManager, GithubPackageManager>();

      var pluginFolder = Path.Combine(AppContext.BaseDirectory, _pluginStorageOptions.Uri); // local plugin cache folder
      var normalizedPath = Path.TrimEndingDirectorySeparator(_pluginStorageOptions.Uri);
      var parent = Path.GetDirectoryName(normalizedPath) ?? string.Empty;
      var shadowFolder = Path.Combine(AppContext.BaseDirectory, parent,"shadow"); //  local plugin shadow copy folder for loading to avoid file lock

      Options.PluginFolder = pluginFolder;
      Options.ShadowFolder = shadowFolder;
      PrepareShadowFolder(pluginFolder, shadowFolder);
      LoadPlugins(shadowFolder);
      return this;
    }
    //public AkkaSyncBuilder AddActorHook<TActor>(string name) where TActor : ActorBase
    //{
    //  Options.HookActors.Add(name, typeof(TActor));
    //  return this;
    //}

    private static void PrepareShadowFolder(string pluginFolder, string shadowFolder)
    {
      if (Directory.Exists(shadowFolder))
      {
        Directory.Delete(shadowFolder, true);
      }
      Directory.CreateDirectory(shadowFolder);
      if (Directory.Exists(pluginFolder))
      {
        var pluginFiles = Directory.GetFiles(pluginFolder, "AkkaSync.Plugins*.zip", SearchOption.TopDirectoryOnly);
        foreach (var pluginFile in pluginFiles)
        {
          if(pluginFile is not null)
          {
            ZipFile.ExtractToDirectory(pluginFile, shadowFolder);
          }
        }
      }
    }

    private AkkaSyncBuilder LoadPlugins(string folder)
    {
      try
      {
        var pluginFiles = Directory.GetFiles(folder, "AkkaSync.Plugins*.dll", SearchOption.AllDirectories);
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
            Options.PluginContexts[file] = context;
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
