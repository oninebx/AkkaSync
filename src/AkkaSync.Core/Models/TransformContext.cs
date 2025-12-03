using System;

namespace AkkaSync.Core.Models;

public class TransformContext
{
  public required string RawData { get; set; }
  public required Dictionary<string, Dictionary<string,object>> TablesData { get; set; }
  public IDictionary<string, object>? MetaData { get; set; }
  public required string Cursor { get; set; }
}
