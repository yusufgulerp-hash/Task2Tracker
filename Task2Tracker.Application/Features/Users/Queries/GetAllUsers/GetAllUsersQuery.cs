using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Users.DTOs;

namespace Task2Tracker.Application.Features.Users.Queries.GetAllUsers;

public sealed record GetAllUsersQuery : IRequest<List<UserListItemDto>>, ICachableQuery
{
    public string CacheKey => "users:all";
    public TimeSpan? Expiration => null;
    public string[] CacheTags => new[] { "users" };
}