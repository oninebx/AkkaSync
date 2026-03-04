using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkkaSync.Infrastructure.Options
{
  public sealed record PluginOptions
  {
    public StorageOptions Storage { get; init; } = new StorageOptions();
    public HashSet<string> CatalogSources { get; init; } = [];
  }
}
