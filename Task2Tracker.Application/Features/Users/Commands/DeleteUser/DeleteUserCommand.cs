using MediatR;
using Task2Tracker.Application.Common.Interfaces;

namespace Task2Tracker.Application.Features.Users.Commands.DeleteUser;

public sealed record DeleteUserCommand(Guid Id)
    : IRequest, ICacheInvalidatingCommand
{
    public string[] CacheTagsToInvalidate => new[] { "users" };
}