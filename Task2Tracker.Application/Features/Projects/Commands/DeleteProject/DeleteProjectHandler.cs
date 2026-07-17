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

    public DeleteProjectCommandHandler(
        IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        IApplicationDbContext context)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _context = context;
    }

    public async Task Handle(
        DeleteProjectCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (project is null)
        {
            throw new NotFoundException("Project not found.");
        }

        var hasOpenTasks = await _taskRepository.HasOpenTasksByProjectIdAsync(
            request.Id,
            cancellationToken);

        if (hasOpenTasks)
        {
            throw new BusinessRuleException(
                "Project which has open task or tasks can't be deleted.");
        }

        _projectRepository.Delete(project);

        await _context.SaveChangesAsync(cancellationToken);
    }
}