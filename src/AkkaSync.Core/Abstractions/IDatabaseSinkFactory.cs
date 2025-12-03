using System;

namespace AkkaSync.Core.Abstractions;

public interface IDatabaseSinkFactory
{
  bool CanCreate(string type);
  ISyncSink Create(string connectionString);
}
