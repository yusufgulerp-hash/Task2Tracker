namespace Task2Tracker.Application.Features.Auth.DTOs;

public sealed record AuthResponseDto(
    string AccessToken,
    Guid UserId,
    string Email,
    string Role);