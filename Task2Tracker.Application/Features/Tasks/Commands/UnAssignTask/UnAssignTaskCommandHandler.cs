using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Tasks.Commands.UnassignTask;

public sealed class UnassignTaskCommandHandler : IRequestHandler<UnassignTaskCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IApplicationDbContext _context;

    public UnassignTaskCommandHandler(
        ITaskRepository taskRepository,
        IApplicationDbContext context)
    {
        _taskRepository = taskRepository;
        _context = context;
    }

    public async Task Handle(
        UnassignTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(
            request.TaskId, cancellationToken);

        if (task is null)
            throw new NotFoundException("Görev bulunamadı.");

        if (task.UserId is null)
            throw new BusinessRuleException("Bu görev zaten hiçbir kullanıcıya atanmamış.");

        task.UnassignUser();

        await _context.SaveChangesAsync(cancellationToken);
    }
}