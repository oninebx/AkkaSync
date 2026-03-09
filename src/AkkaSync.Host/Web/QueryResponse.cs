using System;

namespace AkkaSync.Host.Web;

public sealed record QueryResponse(bool Success, string Message);
