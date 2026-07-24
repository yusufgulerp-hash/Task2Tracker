using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using System;
using System.Threading;
using System.Threading.Tasks;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Common.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly HybridCache _hybridCache;
    private readonly ICurrentUserService _currentUser;

    public CachingBehavior(HybridCache hybridCache, ICurrentUserService currentUser)
    {
        _hybridCache = hybridCache;
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICachableQuery cachableQuery)
        {
            return await next();
        }

        var scope = _currentUser.IsAuthenticated
            ? (_currentUser.IsAdmin ? "admin" : _currentUser.UserId.ToString())
            : "anonymous";

        var scopedCacheKey = $"{cachableQuery.CacheKey}:{scope}";

        var cacheOptions = new HybridCacheEntryOptions
        {
            // Eğer istek içinde özel bir süre belirtilmediyse varsayılan olarak 5 dakika cache'te tut
            Expiration = cachableQuery.Expiration ?? TimeSpan.FromMinutes(5),
            LocalCacheExpiration = cachableQuery.Expiration ?? TimeSpan.FromMinutes(5)
        };


        TResponse response = await _hybridCache.GetOrCreateAsync(
            key: scopedCacheKey,
            factory: async ctx => await next(),
            options: cacheOptions,
            tags: cachableQuery.CacheTags,
            cancellationToken: cancellationToken
        );

        return response;
    }
}