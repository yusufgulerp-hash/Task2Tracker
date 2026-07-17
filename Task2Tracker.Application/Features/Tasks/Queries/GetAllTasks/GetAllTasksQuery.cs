using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Tasks.Queries.GetAllTasks;

public sealed record GetAllTasksQuery : IRequest<List<TaskListItemDto>>, ICachableQuery
{
    public string CacheKey => "tasks:all";
    public TimeSpan? Expiration => null;
    public string[] CacheTags => new[] { "tasks" };
}