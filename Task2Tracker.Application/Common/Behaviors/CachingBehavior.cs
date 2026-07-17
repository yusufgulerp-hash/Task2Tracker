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

    public CachingBehavior(HybridCache hybridCache)
    {
        _hybridCache = hybridCache;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Eğer gelen istek ICachableQuery arayüzünü implement etmiyorsa, cache tüneline girmeden düz geç
        if (request is not ICachableQuery cachableQuery)
        {
            return await next();
        }
        // HybridCache Ayarlarını yapılandırıyoruz
        var cacheOptions = new HybridCacheEntryOptions
        {
            // Eğer istek içinde özel bir süre belirtilmediyse varsayılan olarak 5 dakika cache'te tut
            Expiration = cachableQuery.Expiration ?? TimeSpan.FromMinutes(5),
            LocalCacheExpiration = cachableQuery.Expiration ?? TimeSpan.FromMinutes(5)
        };


        // GetOrCreateAsync: CacheKey'e bakar. RAM'de varsa döndürür, yoksa "underlying factory" (yani next()) metodunu tetikleyip veri tabanından çeker ve cache'ler.
        TResponse response = await _hybridCache.GetOrCreateAsync(
            key: cachableQuery.CacheKey,
            factory: async ctx => await next(),
            options: cacheOptions,
            tags: cachableQuery.CacheTags,
            cancellationToken: cancellationToken
        );

        return response;
    }
}