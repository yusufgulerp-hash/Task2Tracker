using MediatR;
using Task2Tracker.Application.Features.Auth.DTOs;

namespace Task2Tracker.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponseDto>;