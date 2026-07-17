using MediatR;
using Task2Tracker.Application.Common.Exceptions;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Tasks.Queries.GetTaskById;

public sealed class GetTaskByIdQueryHandler
    : IRequestHandler<GetTaskByIdQuery, TaskListItemDto>
{
    private readonly ITaskRepository _taskRepository;

    public GetTaskByIdQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
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