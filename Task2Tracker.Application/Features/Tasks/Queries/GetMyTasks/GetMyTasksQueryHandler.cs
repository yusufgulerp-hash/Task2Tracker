using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Tasks.Queries.GetMyTasks;

public sealed class GetMyTasksQueryHandler
    : IRequestHandler<GetMyTasksQuery, List<TaskListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyTasksQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<TaskListItemDto>> Handle(
        GetMyTasksQuery request,
        CancellationToken cancellationToken)
    {
        // "Benim task'larım" — UserId asla request'ten değil, her zaman
        // token'daki kimlikten geliyor. Böylece bu endpoint'ten başka bir
        // kullanıcının task listesini isteme imkânı yok.
        var query = _context.Tasks
            .AsNoTracking()
            .Where(t => t.UserId == _currentUser.UserId);

        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        if (request.Priority.HasValue)
            query = query.Where(t => t.Priority == request.Priority.Value);

        return await query
            .Select(t => new TaskListItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                ProjectId = t.ProjectId,
                UserId = t.UserId
            })
            .ToListAsync(cancellationToken);
    }
}
