using System;

namespace Task2Tracker.Application.Common.Interfaces;

public interface ICachableQuery
{
    string CacheKey { get; }

    TimeSpan? Expiration { get; }

    string[] CacheTags { get; }
}