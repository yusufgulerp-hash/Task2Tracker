using MediatR;
using Task2Tracker.Application.Common.Interfaces;
using Task2Tracker.Application.Features.Users.DTOs;

namespace Task2Tracker.Application.Features.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDetailDto>, ICachableQuery
{
    public string CacheKey => $"users:{Id}";
    public TimeSpan? Expiration => null;
    public string[] CacheTags => new[] { "users" };
}