using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Projects.DTOs;

namespace Task2Tracker.Application.Features.Projects.Queries.GetAllProjects;

public sealed record GetAllProjectsQuery : IRequest<List<ProjectListItemDto>>, ICachableQuery
{
    public string CacheKey => "projects:all";
    public TimeSpan? Expiration => null;
    public string[] CacheTags => new[] { "projects" };
}