using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Projects.Commands.RemoveProjectMember;

public sealed class RemoveProjectMemberCommandHandler
    : IRequestHandler<RemoveProjectMemberCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RemoveProjectMemberCommandHandler(
        IProjectRepository projectRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _projectRepository = projectRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        RemoveProjectMemberCommand request,
        CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
        {
            throw new ForbiddenException("Projeden üye çıkarma yetkiniz yok.");
        }

        var project = await _projectRepository.GetByIdWithMembersAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            throw new NotFoundException("Proje bulunamadı.");
        }

        var memberToRemove = project.Members
            .FirstOrDefault(m => m.UserId == request.UserId);

        try
        {
            project.RemoveMember(request.UserId);
        }
        catch (InvalidOperationException ex)
        {
            throw new BusinessRuleException(ex.Message);
        }

        if (memberToRemove is not null)
        {
            _context.ProjectMembers.Remove(memberToRemove);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
