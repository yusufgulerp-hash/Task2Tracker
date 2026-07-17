using Task2Tracker.Domain.Enums;

namespace Task2Tracker.WebAPI.Contracts.Tasks;

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    Guid ProjectId);