using System;
using System.Runtime.CompilerServices;
using AkkaSync.Core.Common;
using AkkaSync.Core.Configuration;
using AkkaSync.Core.Pipeline;
using AkkaSync.Core.PluginProviders;
using Microsoft.Extensions.Logging;

namespace AkkaSync.Plugins.Sources;

public class FolderWatcherSourceProvider : IPluginProvider<ISyncSource>
{
  public string Key => nameof(FolderWatcherSourceProvider);
  private readonly ILoggerFactory _factory;

  public FolderWatcherSourceProvider(ILoggerFactory loggerFactory)
  {
    _factory = loggerFactory;
  }

  IEnumerable<ISyncSource> IPluginProvider<ISyncSource>.Create(PipelineContext context, CancellationToken cancellationToken)
  {
    var extension = context.SourceProvider.Parameters["source"];
    var files = Directory.GetFiles(context.SourceProvider.Parameters["folder"], $"*.{extension}");

    foreach (var file in files)
    {
      cancellationToken.ThrowIfCancellationRequested();
      var name = Path.GetFileName(file);
      switch(context.SourceProvider.Parameters["source"])
      {
        case "csv":
          var csvlogger = _factory.CreateLogger<CsvSource>();
          yield return new CsvSource(file, csvlogger);
          break;
        default:
          throw new NotSupportedException($"Source type {context.SourceProvider.Parameters["source"]} is not supported.");
      }
    }
  }
}
