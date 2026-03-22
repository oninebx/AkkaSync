using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.Messaging.Contract.Swap
{
  public static class Protocol
  {
    public sealed record CheckAndUpdatePlugins(IEnumerable<string> Required);
    
    public sealed record ReloadPlugin(string Path);
    public sealed record ReloadPluginInternal(string Path);
    public sealed record CleanupPendingPlugins();

    // PluginLoaderActor
    public sealed record CleanupPlugins();
    public sealed record RestorePlugins();
    public sealed record LoadPlugin(string PluginId);
    public sealed record UnloadPlugin(string PluginId);
  }
}
