using System;

namespace AkkaSync.Core.Messging;

public record StartProcessing();
public record ProcessingCompleted(string Name, string SourceId, string ETag);
public record ProcessingFailed(string Name, string SourceId, string Reason);
public record ProcessingProgress(string SourceId, string Cursor);