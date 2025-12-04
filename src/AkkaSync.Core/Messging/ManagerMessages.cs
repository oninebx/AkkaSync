using System;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Core.Messging;

public record StartPipeline(PipelineContext Context);
public record StopPipeline(string Name);
public record PipelineCompleted(string Name);
