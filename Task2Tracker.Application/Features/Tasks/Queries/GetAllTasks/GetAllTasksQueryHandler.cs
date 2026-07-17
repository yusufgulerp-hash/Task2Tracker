using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Tasks.Queries.GetAllTasks;

public sealed class GetAllTasksQueryHandler
    : IRequestHandler<GetAllTasksQuery, List<TaskListItemDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetAllTasksQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<TaskListItemDto>> Handle(
        GetAllTasksQuery request,
        CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetAllAsync(cancellationToken);

        return tasks
            .Select(task => new TaskListItemDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                ProjectId = task.ProjectId,
                UserId = task.UserId
            })
            .ToList();
    }
}