using Task2Tracker.Domain.Enums;

namespace Task2Tracker.WebAPI.Contracts.Tasks;

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    TaskProgressStatus Status,
    Guid? UserId);