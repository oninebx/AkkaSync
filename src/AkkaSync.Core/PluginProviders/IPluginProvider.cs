using System;
using System.Runtime.CompilerServices;
using AkkaSync.Core.Configuration;
using AkkaSync.Core.Pipeline;

namespace AkkaSync.Core.PluginProviders;

public interface IPluginProvider<T> where T : class
{
  string Key { get; }
  IEnumerable<T> Create(PipelineContext context, CancellationToken cancellationToken);
}
