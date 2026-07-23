using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Tasks.Queries.GetTaskById;

public sealed class GetTaskByIdQueryHandler
    : IRequestHandler<GetTaskByIdQuery, TaskListItemDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetTaskByIdQueryHandler(
        ITaskRepository taskRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _taskRepository = taskRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<TaskListItemDto> Handle(
        GetTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.Id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException("Task not found.");
        }

        if (!_currentUser.IsAdmin)
        {
            var isMember = await _context.ProjectMembers.AnyAsync(
                m => m.ProjectId == task.ProjectId && m.UserId == _currentUser.UserId,
                cancellationToken);

            if (!isMember)
            {
                throw new ForbiddenException(
                    "Bu görevi görme yetkiniz yok — görevin projesine üye değilsiniz.");
            }
        }

        return new TaskListItemDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            ProjectId = task.ProjectId,
            UserId = task.UserId
        };
    }
}