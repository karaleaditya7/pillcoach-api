using System;
using Microsoft.AspNetCore.Builder;

namespace OntrackDb.Hub
{
    public static class WebSocketExtensionMiddlware
    {
        public static IApplicationBuilder UseSocket(
                this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketMiddleware>();
        }
    }
}
