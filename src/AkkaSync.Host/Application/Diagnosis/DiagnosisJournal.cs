using System;
using System.Collections.Immutable;
using AkkaSync.Abstractions;
using AkkaSync.Host.Application.Store;

namespace AkkaSync.Core.Application.Diagnosis;

public sealed record DiagnosisJournal(ImmutableList<DiagnosisRecord> Records) : IStoreValue
{
    public static DiagnosisJournal Empty => new([]);

    private DiagnosisJournal() : this([]) { }
    public DiagnosisJournal AddRecord(DiagnosisRecord record)
    {
        return this with { Records = Records.Add(record) };
    }
}

