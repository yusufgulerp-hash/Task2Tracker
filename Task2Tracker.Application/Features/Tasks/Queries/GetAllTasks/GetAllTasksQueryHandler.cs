using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Tasks.Queries.GetAllTasks;

public sealed class GetAllTasksQueryHandler
    : IRequestHandler<GetAllTasksQuery, List<TaskListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllTasksQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<TaskListItemDto>> Handle(
        GetAllTasksQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Tasks.AsNoTracking();

        if (!_currentUser.IsAdmin)
        {
            var memberProjectIds = _context.ProjectMembers
                .Where(m => m.UserId == _currentUser.UserId)
                .Select(m => m.ProjectId);

            query = query.Where(x => memberProjectIds.Contains(x.ProjectId));
        }

        if (request.ProjectId.HasValue)
            query = query.Where(x => x.ProjectId == request.ProjectId.Value);

        if (request.UserId.HasValue)
            query = query.Where(x => x.UserId == request.UserId.Value);

        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        if (request.Priority.HasValue)
            query = query.Where(x => x.Priority == request.Priority.Value);

        return await query
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
            .ToListAsync(cancellationToken);
    }
}