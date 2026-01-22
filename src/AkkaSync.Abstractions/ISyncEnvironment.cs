using System;

namespace AkkaSync.Abstractions;

public interface ISyncEnvironment
{
  string ComputeSha256(params string[] values);
  string ResolveDataPath(string logicalPath);
  string ResolveConnectionString(string connectionString);
}
