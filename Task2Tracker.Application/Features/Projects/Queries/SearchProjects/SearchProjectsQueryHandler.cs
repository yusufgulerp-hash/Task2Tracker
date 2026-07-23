using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Projects.DTOs;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Projects.Queries.SearchProjects;

public sealed class SearchProjectsQueryHandler
    : IRequestHandler<SearchProjectsQuery, List<ProjectListItemDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SearchProjectsQueryHandler(
        IProjectRepository projectRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _projectRepository = projectRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ProjectListItemDto>> Handle(
        SearchProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.SearchAsync(
            request.Text,
            cancellationToken);

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
            .Select(x => new ProjectListItemDto(
                x.Id,
                x.Name))
            .ToList();
    }
}