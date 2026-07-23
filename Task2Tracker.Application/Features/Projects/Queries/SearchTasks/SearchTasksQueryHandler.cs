using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Tasks.Queries.SearchTasks;

public sealed class SearchTasksQueryHandler
    : IRequestHandler<SearchTasksQuery, List<TaskListItemDto>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SearchTasksQueryHandler(
        ITaskRepository taskRepository,
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _taskRepository = taskRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<TaskListItemDto>> Handle(
        SearchTasksQuery request,
        CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.SearchAsync(
            request.Text,
            cancellationToken);

        if (!_currentUser.IsAdmin)
        {
            var memberProjectIds = await _context.ProjectMembers
                .Where(m => m.UserId == _currentUser.UserId)
                .Select(m => m.ProjectId)
                .ToListAsync(cancellationToken);

            tasks = tasks
                .Where(x => memberProjectIds.Contains(x.ProjectId))
                .ToList();
        }

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