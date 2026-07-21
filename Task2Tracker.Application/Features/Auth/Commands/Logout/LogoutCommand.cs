using MediatR;

namespace Task2Tracker.Application.Features.Auth.Commands.Logout;

public sealed record LogoutCommand(Guid UserId) : IRequest;