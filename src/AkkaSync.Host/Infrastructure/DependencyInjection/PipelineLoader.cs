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
      if(!Directory.Exists(dir))
      {
        throw new DirectoryNotFoundException($"Directory not found: {dir}");
      };
        var builder = new ConfigurationBuilder();

        foreach (var file in Directory.GetFiles(dir, "pipeline_*.json", SearchOption.AllDirectories))
        {
          builder.AddJsonFile(file, optional: false, reloadOnChange: true);
        }

        return builder.Build();
    }
}
