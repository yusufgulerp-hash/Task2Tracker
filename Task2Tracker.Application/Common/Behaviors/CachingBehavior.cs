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
        // Eğer gelen istek ICachableQuery arayüzünü implement etmiyorsa, cache tüneline girmeden düz geç
        if (request is not ICachableQuery cachableQuery)
        {
            return await next();
        }

        // ÖNEMLİ: Cache key'i her zaman isteği atan kullanıcıya göre scope'luyoruz.
        // Aksi halde, sonucu kullanıcıya göre farklılaşan bir query (ör. üyelik
        // filtresi uygulanan GetAllProjects) sabit bir key ile cache'lenirse,
        // kullanıcı A'nın gördüğü sonuç kullanıcı B'ye servis edilebilir —
        // bu bir cache-based veri sızıntısı olur. Bu yüzden "sonucu kullanıcıya
        // göre değişmiyor" diye bilinen query'ler için bile bu scope'lama
        // güvenli bir varsayılan: en kötü ihtimalle biraz daha düşük hit-rate.
        var scope = _currentUser.IsAuthenticated
            ? (_currentUser.IsAdmin ? "admin" : _currentUser.UserId.ToString())
            : "anonymous";

        var scopedCacheKey = $"{cachableQuery.CacheKey}:{scope}";

        // HybridCache Ayarlarını yapılandırıyoruz
        var cacheOptions = new HybridCacheEntryOptions
        {
            // Eğer istek içinde özel bir süre belirtilmediyse varsayılan olarak 5 dakika cache'te tut
            Expiration = cachableQuery.Expiration ?? TimeSpan.FromMinutes(5),
            LocalCacheExpiration = cachableQuery.Expiration ?? TimeSpan.FromMinutes(5)
        };


        // GetOrCreateAsync: CacheKey'e bakar. RAM'de varsa döndürür, yoksa "underlying factory" (yani next()) metodunu tetikleyip veri tabanından çeker ve cache'ler.
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