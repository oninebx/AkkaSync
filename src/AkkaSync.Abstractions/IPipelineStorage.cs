using System;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions;

public interface IPipelineStorage
{
  string Key { get; }
  Task<Dictionary<string, PipelineSpec>> LoadPipelineSpecificationsAsync(CancellationToken cancellationToken = default);
  Task<Dictionary<string, ScheduleSpec>> LoadScheduleSpecificationsAsync(CancellationToken cancellationToken = default);

  // Task SavePipelineAsync(PipelineInfo pipeline, CancellationToken cancellationToken = default);
}
