namespace Task2Tracker.Application.Features.Auth.DTOs;

public sealed record RegisterResponseDto(
    Guid UserId,
    string Email,
    string Status,
    string Message);