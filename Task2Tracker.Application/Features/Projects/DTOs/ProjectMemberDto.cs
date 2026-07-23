namespace Task2Tracker.Application.Features.Projects.DTOs;

public sealed record ProjectMemberDto(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    DateTime JoinedAt);
