using MediatR;
using Task2Tracker.Application.Features.Auth.DTOs;

namespace Task2Tracker.Application.Features.Auth.Commands.Refresh;

public sealed record RefreshCommand(string RefreshToken)
    : IRequest<AuthResponseDto>;