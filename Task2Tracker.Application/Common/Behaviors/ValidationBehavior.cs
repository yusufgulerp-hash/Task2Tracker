using FluentValidation;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Task2Tracker.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    // Sistemdeki ilgili Request için tanımlanmış tüm validator'ları Dependency Injection ile topluyoruz
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            // Doğrulama bağlamını oluşturuyoruz
            var context = new ValidationContext<TRequest>(request);

            // Tüm validator'ları asenkron olarak çalıştırıyoruz
            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            // Oluşan tüm hataları tek bir listede topluyoruz
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            // Eğer tek bir hata bile varsa, isteği kesip ValidationException fırlatıyoruz
            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        // Hata yoksa yoluna devam et, bir sonraki tünele veya Handler'a geç
        return await next();
    }
}