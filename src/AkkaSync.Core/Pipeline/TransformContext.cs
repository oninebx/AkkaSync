using System;

namespace AkkaSync.Core.Pipeline;

public class TransformContext
{
  public required string RawData { get; set; }
  public required Dictionary<string, Dictionary<string,object>> TablesData { get; set; }
  public IDictionary<string, object>? MetaData { get; set; }
}
