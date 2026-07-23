using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Projects.DTOs;

namespace Task2Tracker.Application.Features.Projects.Queries.GetProjectMembers;

public sealed record GetProjectMembersQuery(Guid ProjectId)
    : IRequest<List<ProjectMemberDto>>, ICachableQuery
{
    public string CacheKey => $"project-members:{ProjectId}";
    public TimeSpan? Expiration => null;
    public string[] CacheTags => new[] { $"project-members:{ProjectId}" };
}
