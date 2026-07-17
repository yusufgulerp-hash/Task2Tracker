using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Common.Behaviors;

public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly HybridCache _hybridCache;

    public CacheInvalidationBehavior(HybridCache hybridCache)
    {
        _hybridCache = hybridCache;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        if (request is ICacheInvalidatingCommand invalidatingCommand)
        {
            foreach (var tag in invalidatingCommand.CacheTagsToInvalidate)
            {
                await _hybridCache.RemoveByTagAsync(tag, cancellationToken);
            }
        }

        return response;
    }
}