using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Tasks.Commands.DeleteTask;

public sealed class DeleteTaskCommandHandler
    : IRequestHandler<DeleteTaskCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteTaskCommandHandler(
        ITaskRepository taskRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _taskRepository = taskRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(
        DeleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(
            request.Id, cancellationToken);

        if (task is null)
            throw new NotFoundException("Görev bulunamadı.");

        if (!_currentUser.IsAdmin)
        {
            var callerIsMember = await _context.ProjectMembers.AnyAsync(
                m => m.ProjectId == task.ProjectId && m.UserId == _currentUser.UserId,
                cancellationToken);

            if (!callerIsMember)
                throw new ForbiddenException(
                    "Bu görevi silme yetkiniz yok — görevin projesine üye değilsiniz.");
        }

        if (task.Status != Domain.Enums.TaskProgressStatus.Done)
            throw new BusinessRuleException(
                "Yalnızca tamamlanmış (Done) görevler silinebilir.");

        _taskRepository.Delete(task);

        await _context.SaveChangesAsync(cancellationToken);
    }
}