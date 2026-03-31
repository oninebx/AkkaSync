using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Core.Domain.Workers
{
  public sealed record PluginError(string PluginId, string Message, string Context);
}
