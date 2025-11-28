using System;

namespace AkkaSync.Core.Common;

public static class ProviderHelper
{
  public static string GenerateKey(string name, string path)
  {
      var readablePart = $"{name}:{path}";
      using var sha256 = System.Security.Cryptography.SHA256.Create();
      var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(readablePart));
      var hashString = Convert.ToBase64String(hashBytes);
      return $"{readablePart}|{hashString}";
  }
}
