using MediatR;

namespace Task2Tracker.Application.Features.Users.Commands.ApproveUser;

public sealed record ApproveUserCommand(
    Guid UserId) : IRequest;