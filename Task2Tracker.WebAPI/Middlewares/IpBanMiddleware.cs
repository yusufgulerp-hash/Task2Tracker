public sealed class IpBanMiddleware
{
    private readonly RequestDelegate _next;

    public IpBanMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IIpBanService ipBanService)
    {
        var ipAddress =
            context.Connection.RemoteIpAddress?.ToString();

        if (ipAddress is not null &&
            ipBanService.IsBanned(ipAddress))
        {
            context.Abort();
            return;
        }

        await _next(context);
    }
}