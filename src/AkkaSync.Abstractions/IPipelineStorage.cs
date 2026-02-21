using System;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Abstractions;

public interface IPipelineStorage
{
  string Key { get; }
  Task<(PipelineOptions, ScheduleOptions)> LoadSpecificationsAsync(CancellationToken cancellationToken = default);
  // Task SavePipelineAsync(PipelineInfo pipeline, CancellationToken cancellationToken = default);
}
