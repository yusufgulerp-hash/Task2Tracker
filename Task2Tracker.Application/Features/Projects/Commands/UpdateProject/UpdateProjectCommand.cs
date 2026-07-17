using MediatR;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(Guid Id, string Name)
    : IRequest<Unit>, ICacheInvalidatingCommand 
{
    public string[] CacheTagsToInvalidate => new[] { "projects" };
}