using MediatR;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Projects.Commands.DeleteProject;

public sealed record DeleteProjectCommand(Guid Id)
    : IRequest<Unit>, ICacheInvalidatingCommand
{
    public string[] CacheTagsToInvalidate => new[] { "projects" };
}