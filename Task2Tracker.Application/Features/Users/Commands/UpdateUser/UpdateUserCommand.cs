using MediatR;

namespace Task2Tracker.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email)
    : IRequest;