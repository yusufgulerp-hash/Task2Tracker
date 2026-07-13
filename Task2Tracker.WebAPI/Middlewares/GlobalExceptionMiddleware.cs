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
        Console.WriteLine("GLOBAL MIDDLEWARE ÇALIŞTI");
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            BusinessRuleException => StatusCodes.Status400BadRequest,
            ConflictException => StatusCodes.Status409Conflict,
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var response = new
        {
            StatusCode = statusCode,
            Message = exception.Message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}