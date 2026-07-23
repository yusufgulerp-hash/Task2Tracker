using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Tasks.Commands.AssignTask;

public sealed class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AssignTaskCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(
        AssignTaskCommand request,
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
                    "Bu görevde atama yapma yetkiniz yok — görevin projesine üye değilsiniz.");
        }

        var user = await _userRepository.GetByIdAsync(
            request.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException("Kullanıcı bulunamadı.");

        var assigneeIsMember = await _context.ProjectMembers.AnyAsync(
            m => m.ProjectId == task.ProjectId && m.UserId == request.UserId,
            cancellationToken);

        if (!assigneeIsMember)
            throw new BusinessRuleException(
                "Bu kullanıcı görevin projesinin üyesi değil, atama yapılamaz.");

        task.AssignUser(request.UserId);

        await _context.SaveChangesAsync(cancellationToken);
    }
}