using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Application.Diagnosis;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Workers.Events;
using AkkaSync.Core.Notifications;

namespace AkkaSync.Host.Application.Diagnosis;

public static class DiagnosisReducer
{
  public static DiagnosisJournal Reduce(DiagnosisJournal current, INotificationEvent @event) => @event switch
  {
    PipelineStartReported e => current.AddRecord(new DiagnosisRecord(DiagnosisLevel.Info, $"Pipeline '{e.PipelineId}' started.") { Timestamp = @event.OccurredAt }),
    PipelineCompleteReported e => current.AddRecord(new DiagnosisRecord(DiagnosisLevel.Info, $"Pipeline '{e.PipelineId}' completed.") { Timestamp = @event.OccurredAt }),
    PipelineSkipReported e => current.AddRecord(new DiagnosisRecord(DiagnosisLevel.Warning, $"Pipeline '{e.PipelineId}' is skipped.") {Timestamp = @event.OccurredAt }),
    WorkerStartReported e => current.AddRecord(new DiagnosisRecord(DiagnosisLevel.Info, $"Worker '{e.WorkerId}' started.") { Timestamp = @event.OccurredAt }),
    WorkerFailureReported e => current.AddRecord(new DiagnosisRecord(DiagnosisLevel.Error, e.Message) { Timestamp = @event.OccurredAt }),
    WorkerCompleteReported e => current.AddRecord(new DiagnosisRecord(DiagnosisLevel.Info, $"Worker '{e.WorkerId}' completed.") { Timestamp = @event.OccurredAt }),
    WorkerNonCreationReported e => current.AddRecord(new DiagnosisRecord(DiagnosisLevel.Warning, $"Worker for '{e.SourceName}' is not created.") { Timestamp = @event.OccurredAt }),
    _ => current 
  };
}
