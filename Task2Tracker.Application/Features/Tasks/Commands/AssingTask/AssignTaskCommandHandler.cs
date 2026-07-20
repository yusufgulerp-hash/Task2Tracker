using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Interfaces.Repositories;

namespace Task2Tracker.Application.Features.Tasks.Commands.AssignTask;

public sealed class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IApplicationDbContext _context;

    public AssignTaskCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IApplicationDbContext context)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task Handle(
        AssignTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(
            request.TaskId, cancellationToken);

        if (task is null)
            throw new NotFoundException("Görev bulunamadı.");

        var user = await _userRepository.GetByIdAsync(
            request.UserId, cancellationToken);

        if (user is null)
            throw new NotFoundException("Kullanıcı bulunamadı.");

        task.AssignUser(request.UserId);

        await _context.SaveChangesAsync(cancellationToken);
    }
}