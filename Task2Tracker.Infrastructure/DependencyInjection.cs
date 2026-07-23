using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;
using Task2Tracker.Infrastructure.Auth;
using Task2Tracker.Infrastructure.Persistence;
using Task2Tracker.Infrastructure.Repositories;
using Task2Tracker.Infrastructure.Security.IpBan;

namespace Task2Tracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ==========================
        // Veritabanı
        // ==========================
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // ==========================
        // Repository'ler
        // ==========================
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();

        // ==========================
        // Cache
        // ==========================
        services.AddHybridCache();

        services.AddSingleton<IIpBanService, MemoryIpBanService>();

        // ==========================
        // Current User (HttpContext'ten kimlik bilgisi)
        // ==========================
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // ==========================
        // JWT Ayarları
        // ==========================
        // Options Pattern: appsettings'teki "JwtSettings" bloğunu JwtSettings sınıfına bağla
        services.Configure<JwtSettings>(
            configuration.GetSection("JwtSettings"));

        // Scoped: her HTTP isteği için bir instance
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IPasswordService, JwtService>();

        // ==========================
        // JWT Authentication
        // ==========================
        var jwtSettings = configuration
            .GetSection("JwtSettings")
            .Get<JwtSettings>()!;

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // "sub" claim'i varsayılan olarak ClaimTypes.NameIdentifier'a
                // map'lenebiliyor; bunu kapatıp token'daki claim adlarını
                // olduğu gibi (sub, email, role) kullanıyoruz.
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,        // Expire kontrolü
                    ValidateIssuerSigningKey = true, // İmza kontrolü
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero // Varsayılan 5dk tolerans — sıfırlıyoruz
                };
            });

        return services;
    }
}