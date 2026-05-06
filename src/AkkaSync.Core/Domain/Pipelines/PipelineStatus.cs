using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Core.Domain.Pipelines
{
  public enum PipelineStatus
  {
    Running = 1,
    Success = 2,
    Failed = 3,
    Retrying = 4,
    Skipped = 5,
  }
}
