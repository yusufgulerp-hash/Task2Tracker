using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Projects.Commands.DeleteProject;

public sealed class DeleteProjectCommandHandler
    : IRequestHandler<DeleteProjectCommand>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteProjectCommandHandler(
        IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(
        DeleteProjectCommand request,
        CancellationToken cancellationToken)
    {
        // Proje silme yıkıcı bir işlem — DeleteUser'da olduğu gibi Admin-only.
        if (!_currentUser.IsAdmin)
            throw new ForbiddenException("Proje silme yetkiniz yok.");

        var project = await _projectRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (project is null)
            throw new NotFoundException("Project not found.");

        var hasOpenTasks = await _taskRepository.HasOpenTasksByProjectIdAsync(
            request.Id,
            cancellationToken);

        if (hasOpenTasks)
            throw new BusinessRuleException(
                "Açık (tamamlanmamış) görevleri olan bir proje silinemez.");

        _projectRepository.Delete(project);

        await _context.SaveChangesAsync(cancellationToken);
    }
}