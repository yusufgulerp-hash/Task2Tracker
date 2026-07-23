using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Projects.Commands.AddProjectMember;

public sealed class AddProjectMemberCommandHandler
    : IRequestHandler<AddProjectMemberCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AddProjectMemberCommandHandler(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        AddProjectMemberCommand request,
        CancellationToken cancellationToken)
    {
        // Şimdilik: projeye üye eklemek yalnızca Admin'in yetkisinde.
        // (İleride "proje sahibi de ekleyebilsin" istenirse burası genişletilir.)
        if (!_currentUser.IsAdmin)
        {
            throw new ForbiddenException("Projeye üye ekleme yetkiniz yok.");
        }

        var project = await _projectRepository.GetByIdWithMembersAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            throw new NotFoundException("Proje bulunamadı.");
        }

        var user = await _userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("Kullanıcı bulunamadı.");
        }

        try
        {
            project.AddMember(request.UserId);
        }
        catch (InvalidOperationException ex)
        {
            throw new ConflictException(ex.Message);
        }

        var member = project.Members.Single(m => m.UserId == request.UserId);
        _context.ProjectMembers.Add(member);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
