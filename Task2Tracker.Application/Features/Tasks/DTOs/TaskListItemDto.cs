using Task2Tracker.Domain.Enums;

namespace Task2Tracker.Application.Features.Tasks.DTOs;

public sealed class TaskListItemDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = null!;

    public string? Description { get; init; }

    public TaskProgressStatus Status { get; init; }

    public TaskPriority Priority { get; init; }

    public Guid ProjectId { get; init; }

    public Guid? UserId { get; init; }
}