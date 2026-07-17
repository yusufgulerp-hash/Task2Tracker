namespace Task2Tracker.Application.Common.Interfaces;

public interface ICacheInvalidatingCommand
{
    string[] CacheTagsToInvalidate { get; }
}