using System;

namespace Task2Tracker.Application.Common.Interfaces;

public interface ICachableQuery
{
    // Önbellekte saklanacak benzersiz anahtar (Örn: "project-list" veya "task-123")
    string CacheKey { get; }

    // Önbelleğin ne kadar süre canlı kalacağı (Örn: 5 dakika)
    TimeSpan? Expiration { get; }

    // CRUD tetiklendiğinde ilgili cache verilerini toplu sildirebilmek için etiketler
    string[] CacheTags { get; }
}