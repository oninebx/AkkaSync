using System;

namespace AkkaSync.Abstractions;

public interface IDatabaseSinkFactory
{
  bool CanCreate(string type);
  ISyncSink Create(string connectionString);
}
