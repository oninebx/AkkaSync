using System;

namespace AkkaSync.Core.Messging;

public record DashboardEvent(string Type, object Data);
public record DashboardCommand(string Command, object Payload, string ConnectionId);
public record PipelineCommand(string Command, object Payload, string ReplyTo);
