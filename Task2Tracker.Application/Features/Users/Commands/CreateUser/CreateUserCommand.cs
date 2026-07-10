using MediatR;
namespace Task2Tracker.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email) : IRequest<Guid>;