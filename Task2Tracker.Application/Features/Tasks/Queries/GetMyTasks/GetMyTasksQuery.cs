using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Tasks.DTOs;
using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Application.Features.Tasks.Queries.GetMyTasks;

public sealed record GetMyTasksQuery(
    TaskProgressStatus? Status = null,
    TaskPriority? Priority = null) : IRequest<List<TaskListItemDto>>, ICachableQuery
{
    // Kullanıcı bilgisini key'e katmıyoruz çünkü CachingBehavior zaten
    // her key'i çağıran kullanıcıya göre otomatik scope'luyor.
    public string CacheKey => $"tasks:mine:s={Status}:pr={Priority}";
    public TimeSpan? Expiration => null;
    public string[] CacheTags => new[] { "tasks" };
}
