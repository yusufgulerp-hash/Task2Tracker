using MediatR;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Projects.Commands.CreateProject;

public sealed record CreateProjectCommand(string Name)
    : IRequest<Guid>, ICacheInvalidatingCommand
{
    public string[] CacheTagsToInvalidate => new[] { "projects" };
}