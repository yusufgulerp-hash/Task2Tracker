using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Application.Features.Tasks.Commands.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid Id,
    string Title,
    string? Description,
    TaskPriority Priority,
    TaskProgressStatus Status,
    Guid? UserId) : IRequest, ICacheInvalidatingCommand
{
    public string[] CacheTagsToInvalidate => new[] { "tasks" };
}