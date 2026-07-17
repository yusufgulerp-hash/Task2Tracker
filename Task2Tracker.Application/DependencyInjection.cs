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
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            cfg.AddBehavior(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));

            cfg.AddBehavior(
                     typeof(IPipelineBehavior<,>),
                     typeof(CachingBehavior<,>));

            cfg.AddBehavior(
                typeof(IPipelineBehavior<,>),
                typeof(CacheInvalidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}