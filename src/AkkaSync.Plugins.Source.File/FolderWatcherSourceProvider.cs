using System;
using System.Runtime.CompilerServices;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using AkkaSync.Plugins.Source.File;
using Microsoft.Extensions.Logging;

namespace AkkaSync.Plugins.Sources;

public class FolderWatcherSourceProvider : IPluginProvider<ISyncSource>
{
  public string Key => nameof(FolderWatcherSourceProvider);
  private readonly ILoggerFactory _factory;
  private readonly ISyncEnvironment _environment;

  public FolderWatcherSourceProvider(ISyncEnvironment environment, ILoggerFactory loggerFactory)
  {
    _factory = loggerFactory;
    _environment = environment;
  }

  public IEnumerable<ISyncSource> Create(PluginSpec context, CancellationToken cancellationToken)
  {
    var extension = context.Parameters["source"];
    var path = _environment.ResolveDataPath(context.Parameters["folder"]) ?? context.Parameters["folder"];
    var files = Directory.GetFiles(path, $"*.{extension}");

    foreach (var file in files)
    {
      cancellationToken.ThrowIfCancellationRequested();
      var name = Path.GetFileName(file);
      switch(context.Parameters["source"])
      {
        case "csv":
          var csvlogger = _factory.CreateLogger<CsvSource>();
          yield return new CsvSource(file, _environment, csvlogger);
          break;
        default:
          throw new NotSupportedException($"Source type {context.Parameters["source"]} is not supported.");
      }
    }
  }
}
