using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Projects.DTOs;

public sealed record ProjectMemberWithTasksDto(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    List<TaskListItemDto> Tasks);
