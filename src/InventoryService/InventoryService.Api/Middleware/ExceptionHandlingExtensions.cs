//csharp src/InventoryService/InventoryService.Api/Middleware/ExceptionHandlingExtensions.cs
using Microsoft.AspNetCore.Builder;

namespace InventoryService.Api.Middleware;

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseInventoryExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}