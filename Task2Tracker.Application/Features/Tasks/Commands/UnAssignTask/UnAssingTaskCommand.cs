using MediatR;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Tasks.Commands.UnassignTask;

public sealed record UnassignTaskCommand(Guid TaskId)
    : IRequest, ICacheInvalidatingCommand
{
    public string[] CacheTagsToInvalidate => new[] { "tasks" };
}