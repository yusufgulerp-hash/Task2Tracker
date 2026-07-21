namespace Task2Tracker.Application.Features.Users.DTOs;

public sealed record PendingUserDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    DateTime CreatedAt);