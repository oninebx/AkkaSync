using System;
using System.Collections.Immutable;
using AkkaSync.Abstractions;
using AkkaSync.Host.Application.Dashboard;

namespace AkkaSync.Host.Application.Diagnosing;

public sealed record DiagnosisJournal(ImmutableList<DiagnosisRecord> Records) : IStoreValue
{
  public const int MaxRecords = 100;
  public static DiagnosisJournal Empty => new([]);

  private DiagnosisJournal() : this([]) { }
  public DiagnosisJournal AddRecord(DiagnosisRecord record)
  {
    var updated = Records.Add(record);
    if (updated.Count > MaxRecords)
    {
      updated = updated.RemoveAt(0);
    }
    return this with { Records = updated };
  }

  public DiagnosisJournal ToShow(int maxRecords)
  {
    if (Records.Count <= maxRecords)
    {
      return this;
    }
    var toTake = Records.Skip(Math.Max(0, Records.Count - maxRecords)).ToImmutableList();
    return this with { Records = toTake };
  }
}

