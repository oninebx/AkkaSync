using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Application.Diagnosis;
using AkkaSync.Core.Domain.Pipelines.Events;
using AkkaSync.Core.Domain.Workers.Events;

namespace AkkaSync.Host.Application.Diagnosis;

public static class DiagnosisReducer
{
  public static DiagnosisJournal Reduce(DiagnosisJournal current, ISyncEvent @event) => @event switch
  {
    PipelineStarted e => current.AddRecord(new DiagnosisRecord(DiagnosisLevel.Info, $"Pipeline '{e.PipelineId}' started.") { Timestamp = e.Timestamp }),
    PipelineCompleted e => current.AddRecord(new DiagnosisRecord(DiagnosisLevel.Info, $"Pipeline '{e.PipelineId}' completed.") { Timestamp = e.Timestamp }),
    WorkerStarted e => current.AddRecord(new DiagnosisRecord(DiagnosisLevel.Info, $"Worker '{e.WorkerId}' started.") { Timestamp = e.Timestamp }),
    WorkerFailed e => current.AddRecord(new DiagnosisRecord(DiagnosisLevel.Error, e.Reason) { Timestamp = e.Timestamp }),
    WorkerCompleted e => current.AddRecord(new DiagnosisRecord(DiagnosisLevel.Info, $"Worker '{e.WorkerId}' completed.") { Timestamp = e.Timestamp }),
    _ => current 
  };
}
