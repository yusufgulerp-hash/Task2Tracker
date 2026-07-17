using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;
using Task2Tracker.Domain.Entities;

namespace Task2Tracker.Application.Features.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandHandler
    : IRequestHandler<CreateTaskCommand, Guid>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IApplicationDbContext _context;

    public CreateTaskCommandHandler(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        IApplicationDbContext context)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            throw new NotFoundException("Project not found.");
        }

        var task = new TaskItem(
            request.Title,
            request.Description,
            request.Priority,
            request.ProjectId);

        await _taskRepository.Add(task);

        await _context.SaveChangesAsync(cancellationToken);

        return task.Id;
    }
}