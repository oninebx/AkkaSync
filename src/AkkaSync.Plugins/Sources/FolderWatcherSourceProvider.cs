using System;
using System.Runtime.CompilerServices;
using AkkaSync.Core.Common;
using AkkaSync.Core.Configuration;
using AkkaSync.Core.Pipeline;
using AkkaSync.Core.PluginProviders;

namespace AkkaSync.Plugins.Sources;

public class FolderWatcherSourceProvider : IPluginProvider<ISyncSource>
{
  public string Key => nameof(FolderWatcherSourceProvider);

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
          yield return new CsvSource(file);
          break;
        default:
          throw new NotSupportedException($"Source type {context.SourceProvider.Parameters["source"]} is not supported.");
      }
    }
  }
}
