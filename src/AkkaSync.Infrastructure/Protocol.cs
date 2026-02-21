using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure
{
  public static class Protocol
  {
    public sealed record CheckAndUpdatePlugins(IEnumerable<string> Required);
    public sealed record LoadPlugin(string Path);
    public sealed record UnloadPlugin(string Path);
    public sealed record ReloadPlugin(string Path);
    public sealed record ReloadPluginInternal(string Path);
    public sealed record CleanupPendingPlugins();
  }
}
