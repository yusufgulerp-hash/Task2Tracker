using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Serilog;
using Task2Tracker.Application.Common.Exceptions;

namespace Task2Tracker.WebAPI.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            Log.Error(
                exception,
                "Unhandled exception occurred. Path: {Path}",
                context.Request.Path);

            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            BusinessRuleException => StatusCodes.Status400BadRequest,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            ForbiddenException => StatusCodes.Status403Forbidden,
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            AppException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;

        object response = exception switch
        {
            ValidationException validationException => new
            {
                StatusCode = statusCode,
                Message = validationException.Message,
                Errors = validationException.Errors
            },

            _ => new
            {
                StatusCode = statusCode,
                Message = exception.Message
            }
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}