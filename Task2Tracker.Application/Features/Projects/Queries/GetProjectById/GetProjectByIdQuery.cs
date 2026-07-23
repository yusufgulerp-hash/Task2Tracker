using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Projects.DTOs;

namespace Task2Tracker.Application.Features.Projects.Queries.GetProjectById;

public sealed record GetProjectByIdQuery(Guid Id)
    : IRequest<ProjectDetailDto>, ICachableQuery
{
    public string CacheKey => $"projects:{Id}";
    public TimeSpan? Expiration => null;
    public string[] CacheTags => new[] { "projects", $"project-members:{Id}" };
}
