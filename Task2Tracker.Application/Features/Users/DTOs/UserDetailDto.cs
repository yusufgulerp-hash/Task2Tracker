namespace Task2Tracker.Application.Features.Users.DTOs;

public record UserDetailDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email);