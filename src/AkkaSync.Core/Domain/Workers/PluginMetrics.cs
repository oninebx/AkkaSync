using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Core.Domain.Workers
{
  public sealed record PluginMetrics(string PluginId, int ProcessedCount, int ErrorCount);
}
