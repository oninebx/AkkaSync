using System;
using System.Collections.Immutable;

namespace AkkaSync.Host.Domain.Dashboard.ValueObjects;

public sealed record ErrorJournal : IStoreValue
{
  public ImmutableList<ErrorRecord> Errors { get; init; } = [];

  private ErrorJournal(ImmutableList<ErrorRecord> errors) => Errors = errors;

  public static ErrorJournal Empty => new([]);
}
