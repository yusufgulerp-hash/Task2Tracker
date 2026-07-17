using MediatR;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, string FirstName, string LastName, string Email)
    : IRequest, ICacheInvalidatingCommand
{
    public string[] CacheTagsToInvalidate => new[] { "users" };
}