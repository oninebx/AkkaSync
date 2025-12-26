using System;
using System.Text.Json;

namespace AkkaSync.Host.Application.Query;

public sealed record QueryEnvelope(string Method, JsonElement Payload, bool ReturnImmediately = false);
