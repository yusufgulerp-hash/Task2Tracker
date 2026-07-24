using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Serilog;
using Task2Tracker.Application;
using Task2Tracker.Infrastructure;
using Task2Tracker.Infrastructure.Logging;
using Task2Tracker.Infrastructure.Persistence;
using Task2Tracker.WebAPI.Middlewares;
using Task2Tracker.Application.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// Serilog
// ==========================
builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    SerilogConfiguration.Configure(
        context.Configuration,
        loggerConfiguration);
});

// ==========================
// Katman Servisleri
// ==========================
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// ==========================
// Rate Limiter Servisi
// ==========================
builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = async (context, token) =>
    {
        var clientIp =
            context.HttpContext.Connection.RemoteIpAddress?.ToString();

        if (!string.IsNullOrWhiteSpace(clientIp))
        {
            var ipBanService =
                context.HttpContext.RequestServices
                    .GetRequiredService<IIpBanService>();

            ipBanService.Ban(
                clientIp,
                TimeSpan.FromMinutes(1));

            Log.Warning(
                "IP banned for 1 minute due to rate limit. IP: {ClientIp}",
                clientIp);
        }

        context.HttpContext.Response.StatusCode =
            StatusCodes.Status429TooManyRequests;

        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                status = 429,
                title = "Too Many Requests",
                detail = "IP address temporarily banned for 1 minute."
            },
            cancellationToken: token);
    };

    options.GlobalLimiter =
        PartitionedRateLimiter.Create<HttpContext, string>(
            httpContext =>
            {
                var clientIp =
                    httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: clientIp,
                    factory: _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 300,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        });
            });
});

// ==========================
// CORS (React frontend için)
// ==========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendCorsPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") // Vite dev server varsayılan portu
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ==========================
// ASP.NET Core Servisleri
// ==========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("default", limiterOptions =>
    {
        limiterOptions.PermitLimit = 300;
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        Description = "JWT token'ınızı girin."
    });

    options.AddSecurityRequirement(document => new()
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

var app = builder.Build();

// ==========================
// EF Core Migration
// ==========================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

// ==========================
// Development
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ==========================
// Middleware Pipeline
// ==========================
app.UseGlobalExceptionMiddleware();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("FrontendCorsPolicy");
app.UseMiddleware<IpBanMiddleware>();
app.UseRateLimiter(); 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();