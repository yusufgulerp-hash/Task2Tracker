using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Application.Features.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    string Title,
    string? Description,
    TaskPriority Priority,
    Guid ProjectId) : IRequest<Guid>, ICacheInvalidatingCommand
{
    public string[] CacheTagsToInvalidate => new[] { "tasks" };
}