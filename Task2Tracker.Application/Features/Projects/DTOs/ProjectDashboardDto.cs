using Task2Tracker.Application.Features.Tasks.DTOs;

namespace Task2Tracker.Application.Features.Projects.DTOs;

public sealed record ProjectDashboardDto(
    Guid ProjectId,
    string ProjectName,
    List<ProjectMemberWithTasksDto> Members,
    List<TaskListItemDto> UnassignedTasks);
