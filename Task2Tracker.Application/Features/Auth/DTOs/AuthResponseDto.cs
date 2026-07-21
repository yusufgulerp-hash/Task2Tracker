namespace Task2Tracker.Application.Features.Auth.DTOs;

public sealed record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    Guid UserId,
    string Email,
    string Role);