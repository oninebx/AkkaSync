using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace AkkaSync.Host.Infrastructure.DependencyInjection;

public static class PipelineLoader
{
  public static IConfiguration LoadFromFiles(string dir, params string[] names)
  {
    if (!Directory.Exists(dir))
    {
      throw new DirectoryNotFoundException($"Directory not found: {dir}");
    }
    ;
    var builder = new ConfigurationBuilder();
    
    var allFiles = Directory.GetFiles(dir, "pipeline_*.json", SearchOption.AllDirectories);
    var selectedFiles = allFiles
      .Where(f => names.Contains(Path.GetFileNameWithoutExtension(f), StringComparer.OrdinalIgnoreCase))
      .ToArray();
    if(selectedFiles.Length == 0)
    {
      throw new FileNotFoundException($"No matching pipeline JSON files found in {dir}");
    }
    foreach (var file in selectedFiles)
    {
      builder.AddJsonFile(file, optional: false, reloadOnChange: true);
    }

    return builder.Build();
  }
}
