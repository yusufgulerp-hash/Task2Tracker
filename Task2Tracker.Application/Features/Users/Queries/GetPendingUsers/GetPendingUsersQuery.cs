using MediatR;
using Task2Tracker.Application.Features.Users.DTOs;

namespace Task2Tracker.Application.Features.Users.Queries.GetPendingUsers;

public sealed record GetPendingUsersQuery
    : IRequest<List<PendingUserDto>>;