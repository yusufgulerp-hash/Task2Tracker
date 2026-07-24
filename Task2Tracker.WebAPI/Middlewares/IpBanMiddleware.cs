using System.Text.Json;

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
            // Önceden context.Abort() ile bağlantı hiç HTTP cevabı
            // vermeden kesiliyordu — bu da CORS header'larının (ve
            // her türlü header'ın) hiç gönderilememesine yol açıyordu.
            // Tarayıcı bunu "CORS hatası" gibi raporluyordu, oysa asıl
            // sebep buydu. Artık gerçek bir 403 JSON cevabı dönüyoruz.
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(new
                {
                    StatusCode = 403,
                    Message = "IP adresiniz geçici olarak banlandı, birazdan tekrar deneyin."
                }));

            return;
        }

        await _next(context);
    }
}