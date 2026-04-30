using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions;

public interface IPluginProvider<T> where T : class
{
  string Key { get; }
  string Kind => typeof(T).Name;
  IEnumerable<T> Create(PluginSpec context, CancellationToken cancellationToken = default);
  string Version => this.GetType().Assembly.GetName().Version is Version v ? $"{v.Major}.{v.Minor}.{v.Build}" : "1.0.0";
  string Location => this.GetType().Assembly.GetName().Name!;
}
