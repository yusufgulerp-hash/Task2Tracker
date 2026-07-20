using MediatR;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Users.Commands.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string PasswordHash)
    : IRequest<Guid>, ICacheInvalidatingCommand
{
    public string[] CacheTagsToInvalidate => new[] { "users" };
}