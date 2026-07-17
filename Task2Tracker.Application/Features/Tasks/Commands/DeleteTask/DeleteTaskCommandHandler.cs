using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Tasks.Commands.DeleteTask;

public sealed class DeleteTaskCommandHandler
    : IRequestHandler<DeleteTaskCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IApplicationDbContext _context;

    public DeleteTaskCommandHandler(
        ITaskRepository taskRepository,
        IApplicationDbContext context)
    {
        _taskRepository = taskRepository;
        _context = context;
    }

    public async Task Handle(
        DeleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task not found.");
        }

        _taskRepository.Delete(task);

        await _context.SaveChangesAsync(cancellationToken);
    }
}