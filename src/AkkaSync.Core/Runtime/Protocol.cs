using System;
using Akka.Actor;
using AkkaSync.Abstractions.Models;
using AkkaSync.Core.Domain.Shared;

namespace AkkaSync.Core.Runtime;

public static class RegistryProtocol
{
  public sealed record Initialize(IDictionary<string, PipelineSpec> Pipelines);
  public sealed record CreatePipeline(string Id);
}

