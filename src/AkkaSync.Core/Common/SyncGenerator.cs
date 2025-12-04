using System;
using System.Security.Cryptography;
using System.Text;
using AkkaSync.Abstractions;

namespace AkkaSync.Core.Common;

public class SyncGenerator : ISyncGenerator
{
  public string ComputeSha256(params string[] values)
  {
    if (values == null || values.Length == 0)
    {
      throw new ArgumentException("At least one string must be provided", nameof(values));
    }


    var normalized = values
        .Where(v => !string.IsNullOrWhiteSpace(v))
        .Select(v => v.Trim());

    var input = string.Join(",", normalized);
    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
    return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
  }

}
