using MediatR;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Tasks.Commands.AssignTask;

public sealed record AssignTaskCommand(Guid TaskId, Guid UserId)
    : IRequest, ICacheInvalidatingCommand
{
    public string[] CacheTagsToInvalidate => new[] { "tasks" };
}