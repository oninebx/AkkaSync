using System;
using AkkaSync.Abstractions.Models;

namespace AkkaSync.Core.Messages;

public record StartPipeline(PipelineContext Context);
public record StopPipeline(string Name);
