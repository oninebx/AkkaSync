using System;

namespace AkkaSync.Abstractions;

public interface IPluginLoader
{
  void LoadPlugins(string directory);
}
