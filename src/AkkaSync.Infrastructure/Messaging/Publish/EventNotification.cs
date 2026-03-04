using System;

namespace AkkaSync.Infrastructure.Messaging.Publish;


public sealed record EventNotification(string TypeName, object Payload);
