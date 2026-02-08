using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.DependencyInjection
{
  public sealed record AkkaSyncOptions
  {
    public string PluginFolder { get; set; } = string.Empty;
    public Dictionary<string, Type> HookActors { get; } = new Dictionary<string, Type>();
  }
}
