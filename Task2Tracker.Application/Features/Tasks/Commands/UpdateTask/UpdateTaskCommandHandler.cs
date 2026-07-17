using MediatR;
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

    public UpdateTaskCommandHandler(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IApplicationDbContext context)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _context = context;
    }

    public async Task Handle(
        UpdateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task not found.");
        }

        task.UpdateDetails(request.Title, request.Description);
        task.UpdatePriority(request.Priority);

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
                request.UserId.Value,
                cancellationToken);

            if (user is null)
            {
                throw new NotFoundException("Assigned user not found.");
            }

            task.AssignUser(request.UserId.Value);
        }
        else
        {
            task.UnassignUser();
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}