using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Tasks.Queries.GetTaskById;

public sealed record GetTaskByIdQuery(Guid Id) : IRequest<TaskListItemDto>, ICachableQuery
{
    public string CacheKey => $"tasks:{Id}";
    public TimeSpan? Expiration => null;
    public string[] CacheTags => new[] { "tasks" };
}