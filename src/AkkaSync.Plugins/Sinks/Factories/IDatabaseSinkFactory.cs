using System;
using AkkaSync.Core.Pipeline;

namespace AkkaSync.Plugins.Sinks.Factories;

public interface IDatabaseSinkFactory
{
  bool CanCreate(string type);
  ISyncSink Create(string connectionString);
}
