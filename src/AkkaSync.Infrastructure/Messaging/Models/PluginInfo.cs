using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.Messaging.Models
{
  public sealed record PluginInfo(string Name, string Version)
  {
    public static PluginInfo Empty = new("-", "-");
  }
}
