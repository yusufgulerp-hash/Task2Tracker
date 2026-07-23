using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandHandler
    : IRequestHandler<UpdateProjectCommand, Unit>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateProjectCommandHandler(
        IProjectRepository projectRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _projectRepository = projectRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(
        UpdateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdWithMembersAsync(
            request.Id,
            cancellationToken);

        if (project is null)
        {
            throw new NotFoundException("Project not found.");
        }

        if (!_currentUser.IsAdmin && !project.IsMember(_currentUser.UserId))
        {
            throw new ForbiddenException("Bu projeyi güncelleme yetkiniz yok.");
        }

        project.UpdateDetails(request.Name);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}