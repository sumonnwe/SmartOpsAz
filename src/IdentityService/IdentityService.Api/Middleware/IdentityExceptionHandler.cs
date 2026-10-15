using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentValidation;
using IdentityService.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityService.Api.Middleware;

public sealed class IdentityExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IdentityExceptionHandler> _logger;

    public IdentityExceptionHandler(RequestDelegate next, ILogger<IdentityExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new { error = "An error occurred processing your request." };
        var statusCode = HttpStatusCode.InternalServerError;

        switch (exception)
        {
            case ValidationException validationEx:
                statusCode = HttpStatusCode.BadRequest;
                response = new { error = validationEx.Message };
                break;   

            case AuthenticationException authEx:
                statusCode = HttpStatusCode.Unauthorized;
                response = new { error = authEx.Message };
                break;

            case ArgumentException argEx:
                statusCode = HttpStatusCode.BadRequest;
                response = new { error = argEx.Message };
                break;
        }

        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsJsonAsync(response);
    }
}