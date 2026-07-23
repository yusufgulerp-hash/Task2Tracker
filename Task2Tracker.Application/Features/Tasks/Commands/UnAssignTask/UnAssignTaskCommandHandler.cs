using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Tasks.Commands.UnassignTask;

public sealed class UnassignTaskCommandHandler : IRequestHandler<UnassignTaskCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UnassignTaskCommandHandler(
        ITaskRepository taskRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _taskRepository = taskRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(
        UnassignTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(
            request.TaskId, cancellationToken);

        if (task is null)
            throw new NotFoundException("Görev bulunamadı.");

        if (!_currentUser.IsAdmin)
        {
            var callerIsMember = await _context.ProjectMembers.AnyAsync(
                m => m.ProjectId == task.ProjectId && m.UserId == _currentUser.UserId,
                cancellationToken);

            if (!callerIsMember)
                throw new ForbiddenException(
                    "Bu görevde atama kaldırma yetkiniz yok — görevin projesine üye değilsiniz.");
        }

        if (task.UserId is null)
            throw new BusinessRuleException("Bu görev zaten hiçbir kullanıcıya atanmamış.");

        task.UnassignUser();

        await _context.SaveChangesAsync(cancellationToken);
    }
}