using System;

namespace AkkaSync.Abstractions.Models;

public class SyncHistoryRecord
{
  public string SourceId { get; set; } = default!;
  public string? ETag { get; set; }
  public string? Cursor { get; set; }
  public string Status { get; set; } = "Pending";
  public DateTime LastSyncTimeUtc { get; set; } = DateTime.UtcNow;
  public string? ExtraDataJson { get; set; }
}
