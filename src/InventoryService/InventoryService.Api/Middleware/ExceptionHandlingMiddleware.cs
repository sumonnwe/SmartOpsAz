using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using InventoryService.Api.Models;
using InventoryService.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FluentValidation;

namespace InventoryService.Api.Middleware;

internal sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        var method = context.Request.Method;
        var path = context.Request.Path;

        _logger.LogError(exception, "Unhandled exception for {Method} {Path} TraceId:{TraceId}", method, path, traceId);

        ErrorResponse error;
        int statusCode;

        switch (exception)
        {
            case DomainException de:
                // Map domain codes to HTTP status
                if (!string.IsNullOrWhiteSpace(de.Code) && de.Code.Contains("Duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    statusCode = (int)HttpStatusCode.Conflict;
                }
                else if (!string.IsNullOrWhiteSpace(de.Code) &&
                         (de.Code.Contains("Invalid", StringComparison.OrdinalIgnoreCase) || de.Code.Contains("Validation", StringComparison.OrdinalIgnoreCase)))
                {
                    statusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    statusCode = (int)HttpStatusCode.Conflict;
                }

                error = new ErrorResponse
                {
                    Code = de.Code ?? "Domain.Error",
                    Message = de.Message
                };
                break;

            case ValidationException fv:
                statusCode = (int)HttpStatusCode.BadRequest;
                error = new ErrorResponse
                {
                    Code = "Validation.Error",
                    Message = "One or more validation errors occurred.",
                    Details = fv.Errors // FluentValidation errors (will be serialized)
                };
                break;

            case DbUpdateException dbu:
                // Attempt to detect unique constraint violation (Postgres 23505) or "duplicate" in message
                var inner = dbu.InnerException?.Message ?? dbu.Message;
                if (!string.IsNullOrEmpty(inner) &&
                    (inner.Contains("23505") || inner.Contains("duplicate", StringComparison.OrdinalIgnoreCase) || inner.Contains("unique", StringComparison.OrdinalIgnoreCase)))
                {
                    statusCode = (int)HttpStatusCode.Conflict;
                    error = new ErrorResponse
                    {
                        Code = "Database.UniqueConstraint",
                        Message = "A unique constraint was violated."
                    };
                }
                else
                {
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    error = new ErrorResponse
                    {
                        Code = "Database.Error",
                        Message = "A database error occurred."
                    };
                }
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                error = new ErrorResponse
                {
                    Code = "Internal.Error",
                    Message = _env.IsDevelopment() ? exception.Message : "An unexpected error occurred."
                };

                if (_env.IsDevelopment())
                {
                    error = new ErrorResponse
                    {
                        Code = "Internal.Error",
                        Message = exception.Message,
                        Details = new
                        {
                            exception.StackTrace,
                            exception.GetType().FullName
                        }
                    };
                }
                break;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var payload = JsonSerializer.Serialize(error, options);
        await context.Response.WriteAsync(payload);
    }
}