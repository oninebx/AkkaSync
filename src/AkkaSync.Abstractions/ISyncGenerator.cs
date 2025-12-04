using System;

namespace AkkaSync.Abstractions;

public interface ISyncGenerator
{
  string ComputeSha256(params string[] values);
}
