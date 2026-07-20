using MediatR;
using Microsoft.EntityFrameworkCore;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Tasks.Queries.GetAllTasks;

public sealed class GetAllTasksQueryHandler
    : IRequestHandler<GetAllTasksQuery, List<TaskListItemDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllTasksQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskListItemDto>> Handle(
        GetAllTasksQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Tasks.AsNoTracking();

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