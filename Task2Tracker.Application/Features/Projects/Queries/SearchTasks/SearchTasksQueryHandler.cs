using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Tasks.Queries.SearchTasks;

public sealed class SearchTasksQueryHandler
    : IRequestHandler<SearchTasksQuery, List<TaskListItemDto>>
{
    private readonly ITaskRepository _taskRepository;

    public SearchTasksQueryHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<TaskListItemDto>> Handle(
        SearchTasksQuery request,
        CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.SearchAsync(
            request.Text,
            cancellationToken);

        return tasks
            .Select(x => new TaskListItemDto
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                Status = x.Status,
                Priority = x.Priority,
                ProjectId = x.ProjectId,
                UserId = x.UserId
            })
            .ToList();
    }
}