using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Projects.DTOs;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Features.Projects.Queries.GetProjectById;

public sealed class GetProjectByIdQueryHandler
    : IRequestHandler<GetProjectByIdQuery, ProjectDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProjectByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ProjectDetailDto> Handle(
        GetProjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        var project = await _context.Projects
            .Where(p => p.Id == request.Id)
            .Select(p => new { p.Id, p.Name, p.CreatedAt })
            .FirstOrDefaultAsync(cancellationToken);


        if (project is null)
        {
            throw new NotFoundException("Proje bulunamadı.");
        }

        if (!_currentUser.IsAdmin)
        {
            var isMember = await _context.ProjectMembers.AnyAsync(
                m => m.ProjectId == request.Id && m.UserId == _currentUser.UserId,
                cancellationToken);

            if (!isMember)
            {
                throw new ForbiddenException("Bu projeyi görme yetkiniz yok.");
            }
        }

        var memberCount = await _context.ProjectMembers
            .CountAsync(m => m.ProjectId == request.Id, cancellationToken);

        var taskCount = await _context.Tasks
            .CountAsync(t => t.ProjectId == request.Id, cancellationToken);

        return new ProjectDetailDto(
            project.Id,
            project.Name,
            project.CreatedAt,
            memberCount,
            taskCount);
    }
}
