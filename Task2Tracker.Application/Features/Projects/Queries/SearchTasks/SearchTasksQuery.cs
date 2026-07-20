using MediatR;
using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Tasks.Queries.SearchTasks;

public sealed record SearchTasksQuery(string Text)
    : IRequest<List<TaskListItemDto>>;