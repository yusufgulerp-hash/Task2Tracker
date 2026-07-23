namespace Task2Tracker.Application.Features.Projects.DTOs;

public sealed record ProjectDetailDto(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    int MemberCount,
    int TaskCount);
