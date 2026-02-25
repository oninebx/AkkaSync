using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions;

public interface IPluginProvider<T> where T : class
{
  string Key { get; }
  IEnumerable<T> Create(PluginSpec context, CancellationToken cancellationToken = default);
  Version Version => this.GetType().Assembly.GetName().Version!;
}
