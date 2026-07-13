namespace Task2Tracker.WebAPI.Middlewares;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionMiddleware(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}