using MediatR;
using Task2Tracker.Application.Features.Users.DTOs;

namespace Task2Tracker.Application.Features.Users.Queries.SearchUsers;

public record SearchUsersQuery(string Text)
    : IRequest<List<UserListItemDto>>;