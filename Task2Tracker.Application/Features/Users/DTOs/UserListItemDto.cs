namespace Task2Tracker.Application.Features.Users.DTOs;

public record UserListItemDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email);