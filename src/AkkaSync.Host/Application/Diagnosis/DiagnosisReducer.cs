using System;
using AkkaSync.Abstractions;
using AkkaSync.Core.Application.Diagnosis;
using AkkaSync.Core.Domain.Workers.Events;

namespace AkkaSync.Host.Application.Diagnosis;

public static class DiagnosisReducer
{
  public static DiagnosisJournal Reduce(DiagnosisJournal current, ISyncEvent @event) => @event switch
  {
    WorkerFailed e => current with { Records = [.. current.Records, new DiagnosisRecord(DiagnosisLevel.Error, e.Reason) { Timestamp = e.Timestamp }] },
    _ => current 
  };
}
