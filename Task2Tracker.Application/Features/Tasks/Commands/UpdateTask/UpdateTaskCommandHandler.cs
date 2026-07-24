using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Tasks.Commands.UpdateTask;

public sealed class UpdateTaskCommandHandler
    : IRequestHandler<UpdateTaskCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateTaskCommandHandler(
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
        UpdateTaskCommand request,
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
                    "Bu görevi güncelleme yetkiniz yok — görevin projesine üye değilsiniz.");
        }

        // Done task'ı otomatik InProgress'e çeker, sonra status güncellemesi uygulanır
        task.UpdateDetails(request.Title, request.Description);
        task.UpdatePriority(request.Priority);
        task.SetBlocker(request.BlockerNote);

        if (task.Status != request.Status)
        {
            try
            {
                task.UpdateStatus(request.Status);
            }
            catch (InvalidOperationException ex)
            {
                throw new BusinessRuleException(ex.Message);
            }
        }

        if (request.UserId.HasValue)
        {
            var user = await _userRepository.GetByIdAsync(
                request.UserId.Value, cancellationToken);

            if (user is null)
                throw new NotFoundException("Atanacak kullanıcı bulunamadı.");

            // İş kuralı: bir task, sadece o projenin üyesi olan birine atanabilir.
            var assigneeIsMember = await _context.ProjectMembers.AnyAsync(
                m => m.ProjectId == task.ProjectId && m.UserId == request.UserId.Value,
                cancellationToken);

            if (!assigneeIsMember)
                throw new BusinessRuleException(
                    "Bu kullanıcı görevin projesinin üyesi değil, atama yapılamaz.");

            task.AssignUser(request.UserId.Value);
        }
        else
        {
            task.UnassignUser();
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}