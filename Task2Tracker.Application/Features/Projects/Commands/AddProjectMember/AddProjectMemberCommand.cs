using MediatR;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Projects.Commands.AddProjectMember;

public sealed record AddProjectMemberCommand(Guid ProjectId, Guid UserId)
    : IRequest<Unit>, ICacheInvalidatingCommand
{
    public string[] CacheTagsToInvalidate => new[] { "projects", $"project-members:{ProjectId}" };
}
