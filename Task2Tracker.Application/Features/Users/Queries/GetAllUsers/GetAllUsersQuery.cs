using MediatR;
using Task2Tracker.Application.Features.Users.DTOs;

namespace Task2Tracker.Application.Features.Users.Queries.GetAllUsers;

public record GetAllUsersQuery : IRequest<List<UserListItemDto>>;