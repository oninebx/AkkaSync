using System;

namespace AkkaSync.Core.Abstractions;

public interface ISyncGenerator
{
  string ComputeSha256(params string[] values);
}
