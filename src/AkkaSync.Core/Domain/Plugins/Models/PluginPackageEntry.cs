using System.Text.Json.Serialization;

namespace AkkaSync.Core.Domain.Plugins.Models
{
  public sealed record PluginPackageEntry([property: JsonPropertyName("id")]string QualifiedName, string Version, string Url, string Checksum, string Provider);
}
