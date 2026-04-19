using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Core.Domain.Pipelines
{
  public sealed record PluginInstance(string Id, string Name, string Type, string PipelineId)
  {
    public List<string> Dependencies { get; init; } = [];
  }
}
