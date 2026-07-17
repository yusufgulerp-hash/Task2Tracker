using MediatR;
using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Tasks.Queries.GetTaskById;

public sealed record GetTaskByIdQuery(Guid Id) : IRequest<TaskListItemDto>;