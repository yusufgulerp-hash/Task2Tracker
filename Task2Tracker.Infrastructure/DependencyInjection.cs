using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Infrastructure.Persistence;
using Task2Tracker.Infrastructure.Persistence.Repositories;

namespace Task2Tracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. PostgreSQL ve DbContext Bağlantısı
        // "DefaultConnection" string ifadesini birazdan WebAPI projesindeki appsettings.json içerisine yazacağız.
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, b =>
                b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // 2. Arayüz (Interface) ve Gerçek Sınıf Eşleştirmeleri
        // Application katmanı IApplicationDbContext istediğinde ona ürettiğimiz canlı DbContext'i veriyoruz.
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Generic Repository kaydı (Her entity türü için çalışma zamanında dinamik çözümlenir)
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // 3. .NET 10 Hybrid Cache Bellek Mekanizmasının Sisteme Kaydedilmesi
        services.AddHybridCache();

        return services;
    }
}