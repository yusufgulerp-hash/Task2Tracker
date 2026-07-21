using MediatR;

namespace Task2Tracker.Application.Features.Users.Commands.RejectUser;

public sealed record RejectUserCommand(
    Guid UserId) : IRequest;