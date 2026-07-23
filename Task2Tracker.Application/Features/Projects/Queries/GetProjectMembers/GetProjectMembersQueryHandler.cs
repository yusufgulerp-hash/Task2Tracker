using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Projects.DTOs;

namespace Task2Tracker.Application.Features.Projects.Queries.GetProjectMembers;

public sealed class GetProjectMembersQueryHandler
    : IRequestHandler<GetProjectMembersQuery, List<ProjectMemberDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProjectMembersQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ProjectMemberDto>> Handle(
        GetProjectMembersQuery request,
        CancellationToken cancellationToken)
    {
        var projectExists = await _context.Projects
            .AnyAsync(p => p.Id == request.ProjectId, cancellationToken);

        if (!projectExists)
        {
            throw new NotFoundException("Proje bulunamadı.");
        }

        if (!_currentUser.IsAdmin)
        {
            var isMember = await _context.ProjectMembers.AnyAsync(
                m => m.ProjectId == request.ProjectId && m.UserId == _currentUser.UserId,
                cancellationToken);

            if (!isMember)
            {
                throw new ForbiddenException("Bu projenin üye listesini görme yetkiniz yok.");
            }
        }

        return await _context.ProjectMembers
            .Where(m => m.ProjectId == request.ProjectId)
            .Join(
                _context.Users,
                member => member.UserId,
                user => user.Id,
                (member, user) => new { member, user })
            .OrderBy(x => x.user.FirstName)
            .Select(x => new ProjectMemberDto(
                x.user.Id,
                x.user.FirstName,
                x.user.LastName,
                x.user.Email,
                x.member.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
