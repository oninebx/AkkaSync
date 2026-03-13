using System;

namespace AkkaSync.Abstractions;

public interface ISyncEnvironment
{
  string ComputeSha256(params string[] values);
  string ResolvePath(string logicalPath);
  string ResolveConnectionString(string connectionString);
}
