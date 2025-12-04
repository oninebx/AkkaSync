using System;
using System.Runtime.CompilerServices;
using AkkaSync.Abstractions;
using AkkaSync.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace AkkaSync.Plugins.Sources;

public class FolderWatcherSourceProvider : IPluginProvider<ISyncSource>
{
  public string Key => nameof(FolderWatcherSourceProvider);
  private readonly ILoggerFactory _factory;
  private readonly ISyncGenerator _generator;

  public FolderWatcherSourceProvider(ISyncGenerator generator, ILoggerFactory loggerFactory)
  {
    _factory = loggerFactory;
    _generator = generator;
  }

  IEnumerable<ISyncSource> IPluginProvider<ISyncSource>.Create(PluginContext context, CancellationToken cancellationToken)
  {
    var extension = context.Parameters["source"];
    var files = Directory.GetFiles(context.Parameters["folder"], $"*.{extension}");

    foreach (var file in files)
    {
      cancellationToken.ThrowIfCancellationRequested();
      var name = Path.GetFileName(file);
      switch(context.Parameters["source"])
      {
        case "csv":
          var csvlogger = _factory.CreateLogger<CsvSource>();
          yield return new CsvSource(file, _generator, csvlogger);
          break;
        default:
          throw new NotSupportedException($"Source type {context.Parameters["source"]} is not supported.");
      }
    }
  }
}
