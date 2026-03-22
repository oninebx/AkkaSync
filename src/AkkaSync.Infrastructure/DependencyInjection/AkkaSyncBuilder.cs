using Akka.Actor;
using AkkaSync.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using AkkaSync.Infrastructure.PipelineStorages;
using AkkaSync.Infrastructure.SyncPlugins.Storage;
using System.IO.Compression;
using AkkaSync.Infrastructure.SyncPlugins.PackageManager;
using AkkaSync.Infrastructure.Options;
using AkkaSync.Infrastructure.SyncPlugins.Catalog;

namespace AkkaSync.Infrastructure.DependencyInjection
{
  public sealed class AkkaSyncBuilder
  {
    internal AkkaSyncOptions Options { get; } = new();
    private IServiceCollection _services { get; }
    private StorageOptions _pipelineStorageOptions { get; }
    private StorageOptions _pluginStorageOptions { get; }
    private StorageOptions _pluginCatalogOptions { get;  }  
    public AkkaSyncBuilder(IServiceCollection services, IConfiguration configuration)
    {
      _services = services;

      _services.Configure<PluginOptions>(configuration.GetSection("Plugins"));
      _pluginStorageOptions = configuration.GetSection("Plugins:Storage").Get<StorageOptions>() ?? new StorageOptions();
      _pipelineStorageOptions = configuration.GetSection("PipelineStorage").Get<StorageOptions>() ?? new StorageOptions();
      _pluginCatalogOptions = configuration.GetSection("Plugins:Catalog").Get<StorageOptions>() ?? new StorageOptions();
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
      var pluginFolder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, _pluginStorageOptions.Uri)); // local plugin cache folder
      var normalizedPath = Path.TrimEndingDirectorySeparator(_pluginStorageOptions.Uri);
      var parent = Path.GetDirectoryName(normalizedPath) ?? string.Empty;
      var shadowFolder = Path.Combine(AppContext.BaseDirectory, parent, "shadow"); //  local plugin shadow copy folder for loading to avoid file lock

      // build plugin storage
      IPluginStorage pluginStorage = _pluginStorageOptions.Type switch
      {
        "Local" => new LocalPluginStorage(pluginFolder, shadowFolder),
        _ => throw new NotSupportedException($"Plugin storage type {_pluginStorageOptions.Type} is not supported.")
      };
      _services.AddSingleton(pluginStorage);

      // build plugin catalog
      var pluginCatalogFile = Path.Combine(AppContext.BaseDirectory, _pluginCatalogOptions.Uri);
      IPluginCatalog pluginCatalog = _pluginCatalogOptions.Type switch
      {
        "Json" => new JsonPluginCatalog(pluginCatalogFile),
        _ => throw new NotSupportedException($"Plugin catalog type {_pluginCatalogOptions.Type} is not supported.")
      };
      _services.AddSingleton(pluginCatalog);

      // build plugin package manager
      _services.AddHttpClient<IPluginHttpClient, PluginHttpClient>(client =>
      {
        client.Timeout = TimeSpan.FromSeconds(30);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("AkkaSync-PluginClient/1.0");
      });

      _services.AddSingleton<IPluginPackageManager, GithubPackageManager>();

      Options.PluginFolder = pluginFolder;
      Options.ShadowFolder = shadowFolder;
      PrepareShadowFolder(pluginFolder, shadowFolder);
      return this;
    }

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
            var pluginName = Path.GetFileNameWithoutExtension(pluginFile);
            var targetFolder = Path.Combine(shadowFolder, pluginName);
            Directory.CreateDirectory(targetFolder);
            ZipFile.ExtractToDirectory(pluginFile, targetFolder);
          }
        }
      }
    }
  }
}
