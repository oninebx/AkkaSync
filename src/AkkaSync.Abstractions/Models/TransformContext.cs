using System;
using System.Collections.Generic;

namespace AkkaSync.Abstractions.Models;

public sealed class TransformContext
{
  // public required string RawData { get; set; }
  public required IDictionary<string, Dictionary<string,object?>> TablesData { get; init; }
  public IDictionary<string, object>? MetaData { get; set; }
  public required string Cursor { get; set; }
}

