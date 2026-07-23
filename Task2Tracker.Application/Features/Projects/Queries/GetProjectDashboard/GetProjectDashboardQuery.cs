using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Projects.DTOs;

namespace Task2Tracker.Application.Features.Projects.Queries.GetProjectDashboard;

public sealed record GetProjectDashboardQuery(Guid ProjectId)
    : IRequest<ProjectDashboardDto>, ICachableQuery
{
    public string CacheKey => $"project-dashboard:{ProjectId}";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(1);
    public string[] CacheTags => new[] { "tasks", $"project-members:{ProjectId}" };
}
