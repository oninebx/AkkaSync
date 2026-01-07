using System;

namespace AkkaSync.Abstractions.Models;

public sealed record ScheduleSpec
{
  public required string Pipeline { get; init; }
  public string Cron { get; init; } = "* * * * *";
  public bool Enabled { get; init; } = true;
}
