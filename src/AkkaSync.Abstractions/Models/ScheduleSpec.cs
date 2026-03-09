using System;

namespace AkkaSync.Abstractions.Models;

public sealed record ScheduleSpec
{
  public string Cron { get; init; } = "* * * * *";
  public string? Description { get; init; }
  public bool Enabled { get; init; } = true;
}
