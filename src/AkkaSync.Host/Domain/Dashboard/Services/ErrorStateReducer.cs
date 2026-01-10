using System;
using Akka.Event;
using AkkaSync.Abstractions;
using AkkaSync.Core.Domain.Worker;
using AkkaSync.Host.Domain.Dashboard.ValueObjects;

namespace AkkaSync.Host.Domain.Dashboard.Services;

public static class ErrorStateReducer
{
  public static ErrorJournal Reduce(ErrorJournal current, ISyncEvent @event) => @event switch
  {
    WorkerFailed e => current with { Errors = [.. current.Errors, new ErrorRecord(e.Timestamp, e.Reason)] },
    _ => current 
  };
}
