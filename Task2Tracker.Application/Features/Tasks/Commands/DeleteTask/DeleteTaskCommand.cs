using MediatR;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Tasks.Commands.DeleteTask;

public sealed record DeleteTaskCommand(Guid Id)
    : IRequest, ICacheInvalidatingCommand
{
    public string[] CacheTagsToInvalidate => new[] { "tasks" };
}