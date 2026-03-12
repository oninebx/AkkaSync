using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Abstractions.Models
{
  public record PluginDescriptor(string Holder, string Name, string Version);
}
