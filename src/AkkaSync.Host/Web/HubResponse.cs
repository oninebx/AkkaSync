using System;

namespace AkkaSync.Host.Web;

public sealed record HubResponse(bool Success, string Message);
