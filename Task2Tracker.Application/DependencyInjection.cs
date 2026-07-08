using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Task2Tracker.Application.Common.Behaviors;

namespace Task2Tracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Bu projedeki tüm FluentValidation sınıflarını (Validator) otomatik bulur ve kaydeder
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // MediatR mekanizmasını kaydeder
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // İSTEK TÜNELLERİ (BEHAVIORS)
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        });

        return services;
    }
}