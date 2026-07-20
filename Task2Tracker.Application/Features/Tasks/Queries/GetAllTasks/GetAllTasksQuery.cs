using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Tasks.DTOs;
using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Application.Features.Tasks.Queries.GetAllTasks;

public sealed record GetAllTasksQuery(
    Guid? ProjectId = null,
    Guid? UserId = null,
    TaskProgressStatus? Status = null,
    TaskPriority? Priority = null) : IRequest<List<TaskListItemDto>>, ICachableQuery
{
    public string CacheKey =>
        $"tasks:all:p={ProjectId}:u={UserId}:s={Status}:pr={Priority}";
    public TimeSpan? Expiration => null;
    public string[] CacheTags => new[] { "tasks" };
}