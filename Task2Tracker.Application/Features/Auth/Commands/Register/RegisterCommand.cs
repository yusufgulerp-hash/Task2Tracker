using MediatR;
using Task2Tracker.Application.Features.Auth.DTOs;

namespace Task2Tracker.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password)
    : IRequest<RegisterResponseDto>;