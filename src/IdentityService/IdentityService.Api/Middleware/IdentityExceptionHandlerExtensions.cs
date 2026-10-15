using Microsoft.AspNetCore.Builder;

namespace IdentityService.Api.Middleware;

public static class IdentityExceptionHandlerExtensions
{
    public static IApplicationBuilder UseIdentityExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<IdentityExceptionHandler>();
    }
} 