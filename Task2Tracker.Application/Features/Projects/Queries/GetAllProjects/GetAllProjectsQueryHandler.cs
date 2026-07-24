using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Projects.DTOs;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Projects.Queries.GetAllProjects;

public sealed class GetAllProjectsQueryHandler
    : IRequestHandler<GetAllProjectsQuery, List<ProjectListItemDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllProjectsQueryHandler(
        IProjectRepository projectRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _projectRepository = projectRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ProjectListItemDto>> Handle(
        GetAllProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetAllAsync(cancellationToken);

        if (!_currentUser.IsAdmin)
        {
            var memberProjectIds = await _context.ProjectMembers
                .Where(m => m.UserId == _currentUser.UserId)
                .Select(m => m.ProjectId)
                .ToListAsync(cancellationToken);

            projects = projects
                .Where(p => memberProjectIds.Contains(p.Id))
                .ToList();
        }

        return projects
            .Select(project => new ProjectListItemDto(
                project.Id,
                project.Name))
            .ToList();
    }
}