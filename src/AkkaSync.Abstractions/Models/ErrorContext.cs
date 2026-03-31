using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Abstractions.Models
{
  public sealed record ErrorContext(string PluginId, string Message, string Cursor)
  {
    public bool IsFatal { get; set; } = false;
    public string Context { get; set; } = string.Empty;
  }
}
