using System;
using System.Runtime.CompilerServices;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions;

public interface IPluginProvider<T> where T : class
{
  string Key { get; }
  IEnumerable<T> Create(PluginContext context, CancellationToken cancellationToken = default);
}
