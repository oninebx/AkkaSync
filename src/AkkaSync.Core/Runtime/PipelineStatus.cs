using System;

namespace AkkaSync.Core.Runtime;

public enum PipelineStatus
{
  // Starting,
  Running,
  // Cancelling,
  Completed,
  Failed,
  // Cancelled
}
